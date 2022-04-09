using SoulWorker.ResStructureExtractor.DataTypes.FileInfo;
using SoulWorker.ResStructureExtractor.DataTypes.MemoryInfo;
using SoulWorker.ResStructureExtractor.Extensions;
using SoulWorker.ResStructureExtractor.Utils.FileUtils;
using System.Reflection.PortableExecutable;

namespace SoulWorker.ResStructureExtractor;

internal sealed class Extractor
{
    public async ValueTask<Dictionary<string, string[]>> FromUnpacked()
    {
        var names = await Task
            .WhenAll(TableNameUtils.All(_memory));
        
        var namesInfos = await Task
            .WhenAll(names.Select(async v => new NameMemoryInfo(v.Name, await _headers.AddressFrom(v.Offset))));
        
        var namesUsages = await Task
            .WhenAll(namesInfos.Select(async v => new NameUsageFileInfo(v, await TableNameUtils.UsageOffset(_memory, v.Address))));

        var bodies = await Task.WhenAll(namesUsages
            .Where(v => v.IsValidOffset)
            .Select(async v => new LoopFileInfo(v.Name, await TableBodyUtils.BodyFrom(_memory, v.Offset))));

        var tablesInfos = bodies
            .Select(v => new TableFunctionFileInfo(v.Name, TableBodyUtils.FunctionsFrom(_memory, v.Range)))
            .ToArray();

        var addresses = await Task.WhenAll(tablesInfos
            .SelectMany(v => v.ReadFunctions.Select(v => v.Offset))
            .Distinct()
            .Select(async v => new { Offset = v, RowTypeName = $"{await _headers.AddressFrom(v):X}".ToString() }));

        var addressesInfos = addresses.ToDictionary(k => k.Offset, v => v.RowTypeName);

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