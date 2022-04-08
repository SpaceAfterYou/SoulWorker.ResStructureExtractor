using SoulWorker.ResStructureExtractor.DataTypes.FileInfo;
using SoulWorker.ResStructureExtractor.DataTypes.MemoryInfo;
using SoulWorker.ResStructureExtractor.Extensions;
using SoulWorker.ResStructureExtractor.Utils.FileUtils;
using System.Reflection.PortableExecutable;

namespace SoulWorker.ResStructureExtractor;

internal sealed class Extractor
{
    public Dictionary<string, string[]> FromUnpacked()
    {
        var tablesInfos = TableNameUtils
            .All(_memory)
            .Select(v => new NameMemoryInfo(v.Name, _headers.AddressFrom(v.Offset)))
            .Select(v => new NameUsageFileInfo(v, TableNameUtils.UsageOffset(_memory, v.Address)))
            .Where(v => v.IsValidOffset)
            .Select(v => new LoopFileInfo(v.Name, TableBodyUtils.BodyFrom(_memory, v.Offset)))
            .Select(v => new TableFunctionFileInfo(v.Name, TableBodyUtils.FunctionsFrom(_memory, v.Range)))
            .ToArray();

        var addressesInfos = tablesInfos
            .SelectMany(v => v.ReadFunctions.Select(v => v.Offset))
            .Distinct()
            .Select(v => new { Offset = v, RowTypeName = _headers.AddressFrom(v).ToString() })
            .ToDictionary(k => k.Offset, v => v.RowTypeName);

        return tablesInfos
            .Select(v => new { v.Name, Types = v.ReadFunctions.Select(v => addressesInfos[v.Offset]).ToArray() })
            .ToDictionary(k => k.Name.Value, v => v.Types);
    }

    internal Extractor(string path)
    {
        using var stream = File.OpenRead(path);

        _headers = new PEHeaders(stream);
        _memory = File.ReadAllBytes(path);
    }

    #region Private Methods

    #endregion Private Methods

    #region Private Static Methods

    #endregion Private Static Methods

    #region Private Fields

    private readonly PEHeaders _headers;
    private readonly ReadOnlyMemory<byte> _memory;

    #endregion Private Fields
}