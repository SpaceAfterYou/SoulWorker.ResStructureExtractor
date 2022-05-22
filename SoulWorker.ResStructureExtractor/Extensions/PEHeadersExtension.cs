using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace SoulWorker.ResStructureExtractor.Extensions;

internal static class PEHeadersExtension
{
    internal static ValueTask<int> OffsetFrom(this PEHeaders headers, int address)
    {
        var header = headers.PEHeader;
        if (header is null) throw new ApplicationException("Header not found");

        var section = headers.SectionHeaders.FirstOrDefault(v => IsValidSection(v, (int)header.ImageBase, address));
        
        Debug.WriteLineIf(section.Name is null, "Section not found");
        if (section.Name is null) return ValueTask.FromResult(-1);

        var offset = address - ((int)header.ImageBase + section.VirtualAddress) + section.PointerToRawData;

        Debug.WriteLine($"Offset: {offset:X} from address {address:X}");

        return ValueTask.FromResult(offset);
    }

    internal static int AddressFrom(this PEHeaders headers, int offset)
    {
        var header = headers.PEHeader;
        if (header is null) throw new ApplicationException("Header not found");

        var section = headers.SectionHeaders.FirstOrDefault(v => IsValidSection(v, offset));
        if (section.Name is null) throw new ApplicationException("Section not found");

        var address = offset + section.VirtualAddress - section.PointerToRawData + (int)header.ImageBase;

        Debug.WriteLine($"Address: {address:X} from offset {offset:X}");

        return address;
    }

    #region Private Static Methods

    private static bool IsValidSection(in SectionHeader section, int imageBase, int address)
    {
        address -= imageBase;

        var a = section.VirtualAddress;
        var b = section.VirtualAddress + Math.Max(section.VirtualSize, section.SizeOfRawData);

        return address >= a && address < b;
    }

    private static bool IsValidSection(in SectionHeader section, int offset) =>
        offset >= section.PointerToRawData && offset <= section.PointerToRawData + section.SizeOfRawData;

    #endregion Private Static Methods
}
