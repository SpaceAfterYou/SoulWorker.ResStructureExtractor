using SoulWorker.ResStructureExtractor.DataTypes.FileInfo;
using SoulWorker.ResStructureExtractor.DataTypes.MemoryInfo;

namespace SoulWorker.ResStructureExtractor;

internal readonly struct TableFunctionFileInfo
{
    #region Fields

    internal NameMemoryInfo Name { get; }
    internal IReadOnlyList<TableReadFunctionFileInfo> ReadFunctions { get; }

    #endregion Fields

    #region Constructors

    internal TableFunctionFileInfo(NameMemoryInfo name, IEnumerable<TableReadFunctionFileInfo> readFunctions)
    {
        Name = name;
        ReadFunctions = readFunctions.ToArray();
    }

    #endregion Constructors
}

internal readonly struct TableFunctionMemoryInfo
{
    #region Fields

    internal NameMemoryInfo Name { get; }
    internal IReadOnlyList<TableReadFunctionFileInfo> ReadFunctions { get; }

    #endregion Fields

    #region Constructors

    internal TableFunctionMemoryInfo(NameMemoryInfo name, IEnumerable<TableReadFunctionFileInfo> readFunctions)
    {
        Name = name;
        ReadFunctions = readFunctions.ToArray();
    }

    #endregion Constructors
}
