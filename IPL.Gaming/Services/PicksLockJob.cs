using IPL.Gaming.Common.Enums;
using IPL.Gaming.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IPL.Gaming.Services
{
    /// <summary>
    /// Background job that locks picks for matches whose commence time has passed.
    /// Fires at 3:30 PM and 7:30 PM IST every day.
    /// </summary>
    public class PicksLockJob : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;

        // Target fire times in IST (hours, minutes)
        private static readonly (int Hour, int Minute)[] FireTimes =
        [
            (13, 17), // 3:30 PM IST
            (19, 30), // 7:30 PM IST
        ];

        public PicksLockJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[PicksLockJob] Started. Scheduling runs at 3:30 PM and 7:30 PM IST.");
            ScheduleNextRun();
            return Task.CompletedTask;
        }

        private void ScheduleNextRun()
        {
            var delay = GetDelayUntilNextFireTime();
            Console.WriteLine($"[PicksLockJob] Next run in {delay.TotalMinutes:F0} minute(s).");
            _timer = new Timer(async _ => await OnTimerFired(), null, delay, Timeout.InfiniteTimeSpan);
        }

        private async Task OnTimerFired()
        {
            try
            {
                await LockExpiredPicks();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PicksLockJob] Error during run: {ex.Message}");
            }
            finally
            {
                // Dispose current timer and schedule the next one
                _timer?.Dispose();
                _timer = null;
                ScheduleNextRun();
            }
        }

        private async Task LockExpiredPicks()
        {
            Console.WriteLine("[PicksLockJob] Running picks lock check...");

            using var scope = _serviceProvider.CreateScope();
            var matchStatusService = scope.ServiceProvider.GetRequiredService<IMatchStatusService>();
            var matchService = scope.ServiceProvider.GetRequiredService<IMatchService>();

            var allStatuses = await matchStatusService.GetAllMatchStatuses();
            var readyForPicks = allStatuses
                .Where(s => s.Status == MatchStatus.ReadyForPicks)
                .ToList();

            if (!readyForPicks.Any())
            {
                Console.WriteLine("[PicksLockJob] No matches in ReadyForPicks status.");
                return;
            }

            Console.WriteLine($"[PicksLockJob] Found {readyForPicks.Count} match(es) in ReadyForPicks.");

            var nowIst = DateTime.UtcNow.AddHours(5).AddMinutes(30);
            int locked = 0;

            foreach (var statusRecord in readyForPicks)
            {
                try
                {
                    var match = await matchService.GetMatchById(statusRecord.MatchId);
                    if (match == null)
                    {
                        Console.WriteLine($"[PicksLockJob] Match {statusRecord.MatchId} not found — skipping.");
                        continue;
                    }

                    if (nowIst >= match.MatchCommenceStartDate)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        statusRecord.Status = MatchStatus.PicksClosed;
                        await matchStatusService.UpdateMatchStatus(statusRecord);
                        Console.WriteLine($"[PicksLockJob] Locked: {match.MatchName} (commenced {match.MatchCommenceStartDate:dd MMM HH:mm})");
                        locked++;

                        try
                        {
                            var bettingStatsService = scope.ServiceProvider.GetRequiredService<IBettingStatsService>();
                            await bettingStatsService.CalculateAndUpdateBettingStats(statusRecord.MatchId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[PicksLockJob] Betting stats failed for match {statusRecord.MatchId}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PicksLockJob] Failed to process match {statusRecord.MatchId}: {ex.Message}");
                }
            }

            Console.WriteLine($"[PicksLockJob] Done. Locked {locked} match(es).");
        }

        /// <summary>
        /// Calculates the delay from now (IST) until the next scheduled fire time.
        /// </summary>
        private static TimeSpan GetDelayUntilNextFireTime()
        {
            var nowIst = DateTime.UtcNow.AddHours(5).AddMinutes(30);

            TimeSpan shortest = TimeSpan.MaxValue;

            foreach (var (hour, minute) in FireTimes)
            {
                var candidate = new DateTime(nowIst.Year, nowIst.Month, nowIst.Day, hour, minute, 0);
                if (candidate <= nowIst)
                    candidate = candidate.AddDays(1); // already passed today, try tomorrow

                var delay = candidate - nowIst;
                if (delay < shortest)
                    shortest = delay;
            }

            return shortest;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            Console.WriteLine("[PicksLockJob] Stopped.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
