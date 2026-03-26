using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Common.Models.Repository;
using IPL.Gaming.Database;
using IPL.Gaming.Database.Interfaces;
using IPL.Gaming.MatchSchedule;
using IPL.Gaming.Repository;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Services;
using IPL.Gaming.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

Console.WriteLine("IPL Gaming - Match Schedule Import");
Console.WriteLine("===================================\n");

// Load configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Set up dependency injection
var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(configuration);
var cosmosDbSettings = configuration.GetSection("CosmosDbSettings").Get<CosmosDbSettings>();
services.AddSingleton(cosmosDbSettings);

services.AddScoped<ICosmosService, CosmosService>();
services.AddScoped<IMatchRepository, MatchRepository>();
services.AddScoped<IMatchService, MatchService>();

var serviceProvider = services.BuildServiceProvider();

try
{
    Console.WriteLine("Reading match-schedule-simplified.json...");
    var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "match-schedule-simplified.json");

    if (!File.Exists(jsonFilePath))
    {
        Console.WriteLine($"Error: match-schedule-simplified.json not found at {jsonFilePath}");
        return;
    }

    var jsonContent = File.ReadAllText(jsonFilePath);
    var matches = JsonConvert.DeserializeObject<List<Match>>(jsonContent);

    if (matches == null || !matches.Any())
    {
        Console.WriteLine("No matches found in the schedule.");
        return;
    }

    Console.WriteLine($"Found {matches.Count} matches in the schedule.\n");

    var matchService = serviceProvider.GetRequiredService<IMatchService>();

    int successCount = 0;
    int failureCount = 0;

    foreach (var match in matches)
    {
        try
        {
            var createdMatch = await matchService.CreateMatch(match);
            Console.WriteLine($"✓ Inserted: {createdMatch.MatchName} (ID: {createdMatch.Id})");
            successCount++;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Failed to insert: {match.MatchName}");
            Console.WriteLine($"  Error: {ex.Message}");
            failureCount++;
        }
    }

    Console.WriteLine($"\n===================================");
    Console.WriteLine($"Import completed!");
    Console.WriteLine($"Success: {successCount} matches");
    Console.WriteLine($"Failed:  {failureCount} matches");
    Console.WriteLine($"===================================");
}
catch (Exception ex)
{
    Console.WriteLine($"\nFatal Error: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

