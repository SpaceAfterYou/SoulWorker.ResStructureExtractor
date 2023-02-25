using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAfterYou.SoulWorker.ResStructureExtractor.Test;

public static class Utils
{
    public static string ReadWideString(BinaryReader reader) => Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadUInt16() * 2));
    public static string ReadCharString(BinaryReader reader) => Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadUInt16()));
}

[TestClass]
public sealed class TestRead
{
    public TestContext TestContext { get; set; } = default!;

    [TestMethod]
    public async Task ReadFromUnpackedTest()
    {
        var results = await Extract.FromUnpacked(Path.Combine("Data", "SoulWorker_dump.exe"));
        var files = Directory.EnumerateFiles(Path.Combine("Data", "Res"));

        await Parallel.ForEachAsync(files, async (file, ct) =>
        {
            await using var stream = File.OpenRead(file);
            using var reader = new BinaryReader(stream);

            var name = Path.GetFileNameWithoutExtension(file);

            if (!results.Any(v => string.Equals(v.Key, name, StringComparison.OrdinalIgnoreCase)))
            {
                TestContext.WriteLine($"Types for {name} table aren't parsed");
                return;
            }

            if (_langs.Any(name.EndsWith)) name = name[..^4];

            var item = results.First(v => string.Equals(v.Key, name, StringComparison.OrdinalIgnoreCase));

            for (uint i = 0, count = reader.ReadUInt32(); i < count; ++i)
            {
                foreach (var type in item.Value)
                {
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Byte:
                            reader.ReadByte();
                            break;
                        case TypeCode.SByte:
                            reader.ReadSByte();
                            break;
                        case TypeCode.Int16:
                            reader.ReadInt16();
                            break;
                        case TypeCode.UInt16:
                            reader.ReadUInt16();
                            break;
                        case TypeCode.Int32:
                            reader.ReadInt32();
                            break;
                        case TypeCode.UInt32:
                            reader.ReadUInt32();
                            break;
                        case TypeCode.Single:
                            reader.ReadSingle();
                            break;
                        case TypeCode.String:
                            Utils.ReadWideString(reader);
                            break;
                        default:
                            Assert.Fail($"Unknown typecode for type {type}");
                            break;
                    }
                }
            }

            var crc = Utils.ReadCharString(reader);
        });
    }

    private static readonly string[] _langs = new string[] { "_ENG", "_TWN" };
}
