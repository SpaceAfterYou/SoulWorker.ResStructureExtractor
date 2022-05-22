using SoulWorker.ResStructureExtractor;
using SoulWorker.ResStructureExtractor.Cli;
using System.Text.Json;

var config = new Config(args);
var results = (await Extract.FromUnpacked(config.Path)).ToDictionary(k => k.Key, v => v.Value.Select(v => v.Name).ToArray());

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
