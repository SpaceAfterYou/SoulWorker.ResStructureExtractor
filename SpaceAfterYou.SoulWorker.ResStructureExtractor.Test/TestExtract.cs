using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpaceAfterYou.SoulWorker.ResStructureExtractor.Test;

[TestClass]
public sealed class TestExtract
{
    public TestContext TestContext { get; set; } = default!;

    [TestMethod]
    public async Task FromUnpackedTest()
    {
        var results = await Extract.FromUnpacked(Path.Combine("Data", "SoulWorker_dump.exe"));

        foreach (var file in Directory.EnumerateFiles(Path.Combine("Data", "Res")))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (!results.Any(v => string.Equals(v.Key, name, StringComparison.OrdinalIgnoreCase)))
            {
                TestContext.WriteLine($"Types for {name} table aren't parsed");
            }
        }
    }
}
