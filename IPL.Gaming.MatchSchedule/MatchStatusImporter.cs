using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Models.CosmosDB;
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
            Console.WriteLine("\nIPL Gaming - Match Status Import");
            Console.WriteLine("===================================\n");

            var matches = await _matchService.GetAllMatches();

            if (matches == null || !matches.Any())
            {
                Console.WriteLine("No matches found in the database.");
                return;
            }

            Console.WriteLine($"Found {matches.Count} matches. Creating MatchStatus records...\n");

            int successCount = 0;
            int skippedCount = 0;
            int failureCount = 0;

            foreach (var match in matches)
            {
                try
                {
                    // Skip if a status record already exists for this match
                    var existing = await _matchStatusService.GetMatchStatusByMatchId(match.Id);
                    if (existing != null)
                    {
                        Console.WriteLine($"~ Skipped (already exists): {match.MatchName}");
                        skippedCount++;
                        continue;
                    }

                    var matchStatus = new MatchStatusRecord
                    {
                        MatchId = match.Id,
                        Status = MatchStatus.NotStarted
                    };

                    var created = await _matchStatusService.CreateMatchStatus(matchStatus);
                    Console.WriteLine($"✓ Created: {match.MatchName} → {created.Status} (ID: {created.Id})");
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Failed: {match.MatchName}");
                    Console.WriteLine($"  Error: {ex.Message}");
                    failureCount++;
                }
            }

            Console.WriteLine($"\n===================================");
            Console.WriteLine($"Match Status Import completed!");
            Console.WriteLine($"Created:  {successCount}");
            Console.WriteLine($"Skipped:  {skippedCount}");
            Console.WriteLine($"Failed:   {failureCount}");
            Console.WriteLine($"===================================");
        }
    }
}
