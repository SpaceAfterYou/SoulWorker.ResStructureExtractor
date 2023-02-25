using SpaceAfterYou.SoulWorker.ResStructureExtractor.DataTypes.MemoryInfo;
using System.Diagnostics;

namespace SpaceAfterYou.SoulWorker.ResStructureExtractor.DataTypes.FileInfo;

internal readonly struct NameUsageFileInfo
{
    internal NameMemoryInfo Name { get; }
    internal int Offset { get; }
    internal bool IsValidOffset => Offset != 0;

    internal NameUsageFileInfo(NameMemoryInfo name, int offset)
    {
        Name = name;
        Offset = offset;

        Debug.WriteLineIf(!IsValidOffset, $"{Name.Value} not found");
    }
}
