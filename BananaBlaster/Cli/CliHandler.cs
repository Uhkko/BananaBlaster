using System.Diagnostics;
using BananaBlaster.Parser;
using Microsoft.Z3;

namespace BananaBlaster.Cli;

public static class CliHandler
{
    public static void Execute(CliOptions options)
    {
        var mode = options.Incremental ? "incremental " : "";
        Console.WriteLine($"Processing \"{options.Input}\" with {mode}Bit-Blasting.\n");

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
        if(options.Verbose && result?.Status == Status.SATISFIABLE)
        {
            Console.WriteLine();

            var atomValues = result.Value.GetAtomValues();
            var termValues = result.Value.GetTermValues();

            if(atomValues.Count() > 0)
            {
                Console.WriteLine("Atom values:");
                foreach (var elem in atomValues)
                {
                    Console.WriteLine($"{elem.Key} = {elem.Value}");
                }
            }

            if(termValues.Count() > 0) {
                Console.WriteLine("Term values:");
                foreach (var elem in termValues)
                {
                    Console.WriteLine($"{elem.Key} = {elem.Value}");
                }
            }
        }
    }
}
