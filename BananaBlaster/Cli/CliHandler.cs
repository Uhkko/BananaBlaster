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

        var parsingStartTime = DateTime.Now;
        var formula = FormulaParser.Parse(options.Input, parsingContext);
        var parsingEndTime = DateTime.Now;

        SolverResult? result;
        var solvingStartTime = DateTime.Now;
        if (options.Incremental)
        {
            result = BBContext.SolveIncremental<ExampleStrategy>(formula);
        } else {
            result = BBContext.Solve(formula);
        }
        var solvingEndTime = DateTime.Now;

        if(result is null) throw new UnreachableException();

        Console.WriteLine(result?.Status);
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
