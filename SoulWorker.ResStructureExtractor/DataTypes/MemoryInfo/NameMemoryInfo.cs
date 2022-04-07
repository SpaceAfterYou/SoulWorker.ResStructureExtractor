namespace SoulWorker.ResStructureExtractor.DataTypes.MemoryInfo;

internal readonly struct NameMemoryInfo
{
    #region Fields

    internal string Value { get; }
    internal int Address { get; }

    #endregion Fields

    #region Constructors

    internal NameMemoryInfo(string name, int address)
    {
        Value = name;
        Address = address;
    }

    #endregion Constructors
}
