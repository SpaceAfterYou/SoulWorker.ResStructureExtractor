using System.Text;

namespace SpaceAfterYou.SoulWorker.ResStructureExtractor.Test;

public static class Utils
{
    public static string ReadWideString(BinaryReader reader) => Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadUInt16() * 2));
    public static string ReadCharString(BinaryReader reader) => Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadUInt16()));
}
