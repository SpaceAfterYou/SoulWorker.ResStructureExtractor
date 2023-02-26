using SpaceAfterYou.SoulWorker.ResStructureExtractor.Cli;
using SpaceAfterYou.SoulWorker.ResStructureExtractor;
using System.Text.Json;

var config = new Config(args);
var results = (await Extract.FromUnpacked(config.Path)).ToDictionary(k => k.Key, v => v.Value.Select(v => v.Name).ToArray());

if (config.File is not null)
{
    var opts = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    var output = JsonSerializer.Serialize(results, opts);

    await File.WriteAllTextAsync(config.File, output);
}
else
{
    var opts = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    Console.WriteLine(JsonSerializer.Serialize(results, opts));
}
