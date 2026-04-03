using IPL.Gaming.Common.Enums;
using IPL.Gaming.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IPL.Gaming.Services
{
    /// <summary>
    /// Background job that archives matches that have been marked as Done 
    /// for more than 2 days. Runs daily at 12:00 AM IST (midnight).
    /// </summary>
    public class ArchiveMatchesJob : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;

        // Fire at midnight IST (00:00)
        private const int FireHour = 0;
        private const int FireMinute = 0;

        public ArchiveMatchesJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[ArchiveMatchesJob] Started. Scheduling daily runs at 12:00 AM IST.");
            ScheduleNextRun();
            return Task.CompletedTask;
        }

        private void ScheduleNextRun()
        {
            var delay = GetDelayUntilNextFireTime();
            Console.WriteLine($"[ArchiveMatchesJob] Next run in {delay.TotalHours:F1} hour(s).");
            _timer = new Timer(async _ => await OnTimerFired(), null, delay, Timeout.InfiniteTimeSpan);
        }

        private async Task OnTimerFired()
        {
            try
            {
                await ArchiveOldMatches();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ArchiveMatchesJob] Error during run: {ex.Message}");
            }
            finally
            {
                // Dispose current timer and schedule the next one
                _timer?.Dispose();
                _timer = null;
                ScheduleNextRun();
            }
        }

        private async Task ArchiveOldMatches()
        {
            Console.WriteLine("[ArchiveMatchesJob] Running archive check...");

            using var scope = _serviceProvider.CreateScope();
            var matchStatusService = scope.ServiceProvider.GetRequiredService<IMatchStatusService>();

            var allStatuses = await matchStatusService.GetAllMatchStatuses();
            var doneMatches = allStatuses
                .Where(s => s.Status == MatchStatus.Done && s.CompletedAt.HasValue)
                .ToList();

            if (!doneMatches.Any())
            {
                Console.WriteLine("[ArchiveMatchesJob] No matches in Done status with completion date.");
                return;
            }

            Console.WriteLine($"[ArchiveMatchesJob] Found {doneMatches.Count} match(es) in Done status.");

            var nowUtc = DateTime.UtcNow;
            var twoDaysAgo = nowUtc.AddDays(-2);
            int archived = 0;

            foreach (var statusRecord in doneMatches)
            {
                try
                {
                    // Check if match was completed more than 2 days ago
                    if (statusRecord.CompletedAt!.Value < twoDaysAgo)
                    {
                        await matchStatusService.MarkArchived(statusRecord.MatchId);
                        Console.WriteLine($"[ArchiveMatchesJob] Archived: Match {statusRecord.MatchId} (completed {statusRecord.CompletedAt:dd MMM yyyy})");
                        archived++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ArchiveMatchesJob] Failed to archive match {statusRecord.MatchId}: {ex.Message}");
                }
            }

            Console.WriteLine($"[ArchiveMatchesJob] Done. Archived {archived} match(es).");
        }

        /// <summary>
        /// Calculates the delay from now (IST) until midnight IST.
        /// </summary>
        private static TimeSpan GetDelayUntilNextFireTime()
        {
            var nowIst = DateTime.UtcNow.AddHours(5).AddMinutes(30);
            var nextMidnight = new DateTime(nowIst.Year, nowIst.Month, nowIst.Day, FireHour, FireMinute, 0);

            if (nextMidnight <= nowIst)
                nextMidnight = nextMidnight.AddDays(1); // Already passed midnight today, schedule for tomorrow

            var delay = nextMidnight - nowIst;
            return delay;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            Console.WriteLine("[ArchiveMatchesJob] Stopped.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
