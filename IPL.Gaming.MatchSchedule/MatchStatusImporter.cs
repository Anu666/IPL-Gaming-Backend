using IPL.Gaming.Services.Interfaces;

namespace IPL.Gaming.MatchSchedule
{
    public class MatchStatusImporter
    {
        private readonly IMatchService _matchService;
        private readonly IMatchStatusService _matchStatusService;

        public MatchStatusImporter(IMatchService matchService, IMatchStatusService matchStatusService)
        {
            _matchService = matchService;
            _matchStatusService = matchStatusService;
        }

        public async Task Run()
        {
            Console.WriteLine("\nIPL Gaming - Match Status: Backfill MatchCommenceStartDate");
            Console.WriteLine("===================================\n");

            var matches = await _matchService.GetAllMatches();

            if (matches == null || !matches.Any())
            {
                Console.WriteLine("No matches found in the database.");
                return;
            }

            Console.WriteLine($"Found {matches.Count} matches. Updating MatchStatus records with MatchCommenceStartDate...\n");

            int updatedCount = 0;
            int failureCount = 0;

            foreach (var match in matches)
            {
                try
                {
                    var existing = await _matchStatusService.GetMatchStatusByMatchId(match.Id);
                    if (existing == null)
                    {
                        Console.WriteLine($"~ No status record found for: {match.MatchName} — skipping");
                        continue;
                    }

                    existing.MatchCommenceStartDate = match.MatchCommenceStartDate;
                    await _matchStatusService.UpdateMatchStatus(existing);
                    Console.WriteLine($"✓ Updated: {match.MatchName} → {match.MatchCommenceStartDate:yyyy-MM-dd HH:mm}");
                    updatedCount++;

                    // -------------------------------------------------------
                    // Old "create new record" logic — commented out because all
                    // MatchStatus records already exist in the database.
                    // -------------------------------------------------------
                    //var matchStatus = new MatchStatusRecord
                    //{
                    //    MatchId = match.Id,
                    //    Status = MatchStatus.NotStarted
                    //};
                    //var created = await _matchStatusService.CreateMatchStatus(matchStatus);
                    //Console.WriteLine($"✓ Created: {match.MatchName} → {created.Status} (ID: {created.Id})");
                    // -------------------------------------------------------
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Failed: {match.MatchName}");
                    Console.WriteLine($"  Error: {ex.Message}");
                    failureCount++;
                }
            }

            Console.WriteLine($"\n===================================");
            Console.WriteLine($"Backfill completed!");
            Console.WriteLine($"Updated: {updatedCount}");
            Console.WriteLine($"Failed:  {failureCount}");
            Console.WriteLine($"===================================");
        }
    }
}
