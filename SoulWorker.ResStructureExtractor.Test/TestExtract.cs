using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SoulWorker.ResStructureExtractor.Test;

[TestClass]
public sealed class TestExtract
{
	public TestContext TestContext { get; set; } = default!;

	[TestMethod]
	public async Task FromUnpackedTest()
	{
		var results = await Extract.FromUnpacked(@"Data\SoulWorker_dump.exe");

		foreach (var file in Directory.EnumerateFiles(@"Data\Res"))
		{
			var name = Path.GetFileNameWithoutExtension(file);
			if (!results.Any(v => string.Equals(v.Key, name, StringComparison.OrdinalIgnoreCase)))
			{
				TestContext.WriteLine($"Types for {name} table aren't parsed");
			}
		}
	}
}
