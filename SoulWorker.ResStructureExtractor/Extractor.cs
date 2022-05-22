using SoulWorker.ResStructureExtractor.DataTypes.FileInfo;
using SoulWorker.ResStructureExtractor.DataTypes.MemoryInfo;
using SoulWorker.ResStructureExtractor.Extensions;
using SoulWorker.ResStructureExtractor.Utils.FileUtils;
using System.Reflection.PortableExecutable;

namespace SoulWorker.ResStructureExtractor;

internal sealed class Extractor
{
	public Dictionary<string, Type[]> FromUnpacked()
	{
		var tablesInfos = TableNameUtils
			.All(_memory)
			.AsParallel()
			.Select(v =>
			{
				var info = new NameMemoryInfo(v.Name, _headers.AddressFrom(v.Offset));

				var usage = new NameUsageFileInfo(info, TableNameUtils.UsageOffset(_memory, info.Address));
				if (!usage.IsValidOffset) return null;

				var body = new LoopFileInfo(usage.Name, TableBodyUtils.BodyFrom(_memory, usage.Offset));
				var tableInfo = new TableFunctionFileInfo(body.Name, TableBodyUtils.FunctionsFrom(_memory, body.Range));

				return tableInfo;
			})
			.Where(v => v is not null)
			.ToArray();

		var fieldTypes = tablesInfos
				.SelectMany(v => v.ReadFunctions.Select(v => v.Offset))
				.Distinct()
				.Select(v => new { Offset = v, FieldTypeName = GetFieldType(v) });

		var filedInfos = fieldTypes.ToDictionary(k => k.Offset, v => v.FieldTypeName);

		return tablesInfos
				.Select(v => new { v.Name, Types = v.ReadFunctions.Select(v => filedInfos[v.Offset]).ToArray() })
				.ToDictionary(k => k.Name.Value, v => v.Types);
	}

	internal static async ValueTask<Extractor> Create(string path)
	{
		await using var stream = File.OpenRead(path);

		return new Extractor(path, new PEHeaders(stream));
	}

	#region Private Methods

	private Type GetFieldType(int begin)
	{
		var pattern = new ReadOnlyMemory<byte?>(new byte?[] { 0x8B, 0xE5, 0x5D, 0xC2, null, 0x00, 0xCC });

		var end = TableBodyUtils.OffsetByPattern(_memory[begin..], pattern);
		var memory = _memory[new Range(begin, begin + end)];

		var type = _fieldTypes.FirstOrDefault((v) => TableBodyUtils.ContainsPattern(memory, v.Pattern))?.Type;

		return type ?? typeof(string);
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
						new (typeof(sbyte), new byte?[] { 0x8B, 0x55, 0x08, 0x8A, 0x45, 0xFF, 0x88, 0x02, 0x0F, 0xBE, 0x45, 0xFF, 0x99, 0x8B, 0x4D }), // ReadSByte
            new (typeof(short), new byte?[] { 0x8B, 0x45, 0x08, 0x66, 0x8B, 0x4D, 0xFC, 0x66, 0x89, 0x08, 0x0F, 0xBF, 0x45, 0xFC, 0x99 }), // ReadInt16
            new (typeof(ushort), new byte?[] { 0x8B, 0x45, 0x08, 0x66, 0x8B, 0x4D, 0xFC, 0x66, 0x89, 0x08, 0x0F, 0xB7, 0x45, 0xFC, 0x99 }), // ReadUInt16
            new (typeof(int), new byte?[] { 0x8B, 0x55, 0x08, 0x8B, 0x45, 0xFC, 0x89, 0x02, 0x8B, 0x45, 0xFC, 0x99 }), // ReadInt32
            new (typeof(uint), new byte?[] { 0x8B, 0x55, 0x08, 0x8B, 0x45, 0xFC, 0x89, 0x02, 0x8B, 0x4D, 0xFC, 0x33, 0xD2 }), // ReadUInt32
            new (typeof(float), new byte?[] { 0x8B, 0x4D, 0xF8, 0x89, 0x41, 0x08, 0x89, 0x51, 0x0C, 0xD9, 0x45, 0xFC }), // ReadSingle
																																																												 // new (typeof(string), new byte?[] { }), // ReadString
			};

	#endregion Private Static Fields
}
