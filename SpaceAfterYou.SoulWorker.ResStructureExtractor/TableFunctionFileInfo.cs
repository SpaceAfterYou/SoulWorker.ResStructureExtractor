using SpaceAfterYou.SoulWorker.ResStructureExtractor.DataTypes.FileInfo;
using SpaceAfterYou.SoulWorker.ResStructureExtractor.DataTypes.MemoryInfo;

namespace SpaceAfterYou.SoulWorker.ResStructureExtractor;

internal sealed record TableFunctionFileInfo
{
    internal NameMemoryInfo Name { get; }
    internal IReadOnlyList<TableReadFunctionFileInfo> ReadFunctions { get; }

    internal TableFunctionFileInfo(NameMemoryInfo name, IEnumerable<TableReadFunctionFileInfo> readFunctions)
    {
        Name = name;
        ReadFunctions = readFunctions.ToArray();
    }
}
