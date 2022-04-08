using SoulWorker.ResStructureExtractor;
using SoulWorker.ResStructureExtractor.Cli;
using System.Text.Json;

var config = new Config(args);
var results = Extract.FromUnpacked(config.Path);

if (config.File is not null)
{
    var json = JsonSerializer.Serialize(results);

    await File.WriteAllTextAsync(config.File, json);
}
else
{
    var opts = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    Console.WriteLine(JsonSerializer.Serialize(results, opts));
}
