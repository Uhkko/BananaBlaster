using System;

namespace BananaBlaster.Cli;

public static class CliHandler
{
    public static void Execute(CliOptions options)
    {
        if (options.Verbose)
        {
            Console.WriteLine("[DEBUG] Parsed options:");
            Console.WriteLine($"  Input       = {options.Input}");
            Console.WriteLine($"  Incremental = {options.Incremental}");
        }

        var mode = options.Incremental ? "incremental " : "";
        Console.WriteLine($"Processing \"{options.Input}\" with {mode}Bit-Blasting.");

        // TODO: Processing logic in here.
    }
}
