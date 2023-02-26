namespace SpaceAfterYou.SoulWorker.ResStructureExtractor;

public static class Extract
{
    public static Dictionary<string, Type[]> FromUnpacked(byte[] buffer)
    {
        var extractor = Extractor.Create(buffer);
        return extractor.FromUnpacked();
    }

    public static async ValueTask<Dictionary<string, Type[]>> FromUnpacked(string path)
    {
        var extractor = await Extractor.Create(path);
        return extractor.FromUnpacked();
    }
}
