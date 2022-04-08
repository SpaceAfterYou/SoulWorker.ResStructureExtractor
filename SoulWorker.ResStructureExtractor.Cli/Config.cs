namespace SoulWorker.ResStructureExtractor.Cli;

public sealed class Config
{
    #region Properties

    public string Path { get; } = string.Empty;
    public string? File { get; }

    #endregion

    #region Constructors

    public Config(string[] args)
    {
        for (int q = 0; q < args.Length; ++q)
        {
            if (args[q] == "-path")
            {
                Path = args[++q];
                continue;
            }

            if (args[q] == "-file")
            {
                File = args[++q];
                continue;
            }
        }

        if (string.IsNullOrEmpty(Path))
            throw new ApplicationException("Bad file path");
    }

    #endregion Private Constructors
}

