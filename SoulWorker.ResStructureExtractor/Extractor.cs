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
            .Select(async v => new { Offset = v, FieldTypeName = (await GetFieldType(v)).Name }));

        var addressesInfos = addresses.ToDictionary(k => k.Offset, v => v.FieldTypeName);

        return tablesInfos
            .Select(v => new { v.Name, Types = v.ReadFunctions.Select(v => addressesInfos[v.Offset]).ToArray() })
            .ToDictionary(k => k.Name.Value, v => v.Types);
    }

    internal static async ValueTask<Extractor> Create(string path)
    {
        await using var stream = File.OpenRead(path);

        return new Extractor(path, new PEHeaders(stream));
    }

    #region Private Methods

    private async ValueTask<Type> GetFieldType(int begin)
    {
        var pattern = new ReadOnlyMemory<byte?>(new byte?[] { 0x8B, 0xE5, 0x5D, 0xC2, null, 0x00, 0xCC });

        var end = await TableBodyUtils.OffsetByPatternAsync(_memory[begin..], pattern);
        var memory = _memory[new Range(begin, begin + end)];

        return _fieldTypes.FirstOrDefault((v) => TableBodyUtils.ContainsPattern(memory, v.Pattern))?.Type ?? typeof(string);
    }

    private Extractor(string path, PEHeaders headers)
    {
        _headers = headers;
        _memory = File.ReadAllBytes(path);
    }

    #endregion Private Methods

    #region Private Fields

    private readonly PEHeaders _headers;
    private readonly ReadOnlyMemory<byte> _memory;

    #endregion Private Fields

    #region Private Static Fields

    private static readonly IReadOnlyCollection<FieldType> _fieldTypes = new FieldType[]
        {
            new (typeof(byte), new byte?[] { 0x8B, 0x55, 0x08, 0x8A, 0x45, 0xFF, 0x88, 0x02, 0x0F, 0xB6, 0x45, 0xFF, 0x99 }), // ReadByte
            new (typeof(short), new byte?[] { 0x8B, 0x45, 0x08, 0x66, 0x8B, 0x4D, 0xFC, 0x66, 0x89, 0x08, 0x0F, 0xBF, 0x45, 0xFC, 0x99 }), // ReadInt16
            new (typeof(ushort), new byte?[] { 0x8B, 0x45, 0x08, 0x66, 0x8B, 0x4D, 0xFC, 0x66, 0x89, 0x08, 0x0F, 0xB7, 0x45, 0xFC, 0x99 }), // ReadUInt16
            new (typeof(int), new byte?[] { 0x8B, 0x55, 0x08, 0x8B, 0x45, 0xFC, 0x89, 0x02, 0x8B, 0x45, 0xFC, 0x99 }), // ReadInt32
            new (typeof(uint), new byte?[] { 0x8B, 0x55, 0x08, 0x8B, 0x45, 0xFC, 0x89, 0x02, 0x8B, 0x4D, 0xFC, 0x33, 0xD2 }), // ReadUInt32
            new (typeof(float), new byte?[] { 0x8B, 0x4D, 0xF8, 0x89, 0x41, 0x08, 0x89, 0x51, 0x0C, 0xD9, 0x45, 0xFC }), // ReadSingle
            // new (typeof(string), new byte?[] { }), // ReadString
        };

    #endregion Private Static Fields
}
