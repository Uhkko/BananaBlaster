using System.Diagnostics;
using BananaBlaster.Cli.Format;
using BananaBlaster.Parser;
using Microsoft.Z3;

namespace BananaBlaster.Cli;

public static class CliHandler
{
    public static void Execute(CliOptions options)
    {
        if(options.Verbose)
        {
            var mode = options.Incremental ? "incremental " : "";
            Console.WriteLine($"Processing \"{options.Input}\" with {mode}Bit-Blasting.\n");
        }

        var parsingContext = new ParsingContext {
            DefaultSize = options.DefaultSize,
            Overflow = options.Overflow,
        };

        var parsingStartTime = DateTime.Now;
        var formula = FormulaParser.Parse(options.Input, parsingContext);
        var parsingEndTime = DateTime.Now;

        SolverResult? result;
        var solvingStartTime = DateTime.Now;
        if (options.Incremental)
        {
            if (options.Strategy != null) {
                result = BBContext.SolveIncremental<CustomizableStrategy, List<Type>>(formula, options.Strategy);
            } else {
                result = BBContext.SolveIncremental<ExampleStrategy>(formula);
            }
        } else {
            result = BBContext.Solve(formula);
        }
        var solvingEndTime = DateTime.Now;

        if(result is null) throw new UnreachableException();

        Console.WriteLine($"{(options.Verbose ? "Status: " : "")}{SatisfiabilityString(result.Value.Status)}");

        if (options.Verbose)
        {
            Console.WriteLine();

            var parsingDuration = parsingEndTime.Subtract(parsingStartTime);
            var solvingDuration = solvingEndTime.Subtract(solvingStartTime);
            Console.WriteLine($"Parsing Time: {parsingDuration.TotalSeconds}s");
            Console.WriteLine($"Solving Time: {solvingDuration.TotalSeconds}s");
            
            Console.WriteLine();
            Console.WriteLine($"Variable Count: {result.Value.VariableCount}");
            Console.WriteLine($"Operator Count: {result.Value.OperatorCount}");
        }

        if (options.Verbose && result?.Status == Status.SATISFIABLE)
        {
            var atomValues = result.Value.GetAtomValues();
            var termValues = result.Value.GetTermValues();

            Console.WriteLine();      // blank line before the block
            
            if (atomValues.Any() && termValues.Any())
            {
                string output = SideBySideRenderer.Render(
                    leftData:  atomValues,
                    rightData: termValues,
                    leftTitle:  "Atom values:",
                    rightTitle: "Term values:",
                    cap: options.ValueCap,
                    gap: 6
                );
                Console.WriteLine(output);
            }
            else if (atomValues.Any())
            {
                Console.WriteLine(" \x1B[4mAtom values:\x1B[0m");
                Console.WriteLine(TableFormatter.FormatTable(atomValues, cap: options.ValueCap));
            }
            else if (termValues.Any())
            {
                Console.WriteLine(" \x1B[4mTerm values:\x1B[0m");
                Console.WriteLine(TableFormatter.FormatTable(termValues, cap: options.ValueCap));
            }
        }

        if (options.SMTLibOutput is not null)
        {
            File.WriteAllText(options.SMTLibOutput, (result ?? throw new UnreachableException()).SMTLibCode);
            Console.WriteLine();
            Console.WriteLine($"Exported SMT-Lib code to {options.SMTLibOutput}");
        }
    }

    private static string SatisfiabilityString(Status status)
    {
        return status switch
        {
            Status.UNSATISFIABLE => "\x1B[31mUNSATISFIABLE\x1B[0m",
            Status.UNKNOWN => "UNKNOWN",
            Status.SATISFIABLE => "\x1B[32mSATISFIABLE\x1B[0m",
            _ => throw new UnreachableException(),
        };
    }
}
