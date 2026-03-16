namespace BananaBlaster.Cli;

public sealed class CliOptions
{
    public string Input { get; init; } = string.Empty;   // positional argument
    public bool Incremental { get; init; }
    public bool Verbose { get; init; }

    public int DefaultSize { get; init; }
}