namespace BananaBlaster.Cli;

public sealed class CliOptions
{
    public string Input { get; init; } = string.Empty;   // positional argument
    public bool Incremental { get; init; }
    public bool Verbose { get; init; }
    public bool Overflow { get; init; }
    public int DefaultSize { get; init; }
    public int ValueCap { get; init; }
    public string? SMTLibOutput { get; init; }
}
