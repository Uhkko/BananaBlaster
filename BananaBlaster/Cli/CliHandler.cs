using System.Diagnostics;
using BananaBlaster.Parser;
using Microsoft.Z3;

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

        var parsingContext = new ParsingContext {
            DefaultSize = options.DefaultSize,
            Overflow = options.Overflow,
        };

        var formula = FormulaParser.Parse(options.Input, parsingContext);

        SolverResult? result;
        if (options.Incremental)
        {
            result = BBContext.SolveIncremental<ExampleStrategy>(formula);
        } else {
            result = BBContext.Solve(formula);
        }

        if(result is null) throw new UnreachableException();

        Console.WriteLine(result?.Status);
    }
}
