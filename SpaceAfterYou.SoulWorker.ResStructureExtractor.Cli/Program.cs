using SpaceAfterYou.SoulWorker.ResStructureExtractor.Cli;
using SpaceAfterYou.SoulWorker.ResStructureExtractor;
using System.Text.Json;

args = new string[] { "-path", "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Soulworker_KR\\SoulWorker_dump.exe", "-file", "output.json" };

var config = new Config(args);
var results = Extract.FromUnpacked(config.Path).ToDictionary(k => k.Key, v => v.Value.Select(v => v.Name).ToArray());

if (config.File is not null)
{
    var opts = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    var json = JsonSerializer.Serialize(results, opts);

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
