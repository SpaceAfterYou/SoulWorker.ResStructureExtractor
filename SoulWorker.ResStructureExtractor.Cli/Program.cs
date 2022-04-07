using SoulWorker.ResStructureExtractor;
using SoulWorker.ResStructureExtractor.Cli;
using System.Text.Json;

var config = new Config(args);
var results = Extract.FromUnpacked(config.Path);

if (!string.IsNullOrEmpty(config.File))
{
    if (config.Json)
    {
        var json = JsonSerializer.Serialize(results, new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        await File.WriteAllTextAsync(config.File, json);
    }
    else
    {
        //using var file = File.Open(config.File, FileMode.Create);

        //var serializer = new XmlSerializer(results.GetType());
        //var ms = new MemoryStream();
        //serializer.Serialize(ms, results);

        //ms.WriteTo(file);
        //// await File.WriteAllBytesAsync(config.File, ms);
    }
}
