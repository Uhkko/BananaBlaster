using System.CommandLine;

namespace BananaBlaster.Cli;

public static class CliBuilder
{
    public static Command BuildRootCommand()
    {
        var root = new RootCommand("The CLI for the BananaBlaster");

        // Positional arguments
        var inputArg = new Argument<string>("formula")
        {
            Description = "The formula to be evaluated"
        };
        root.Arguments.Add(inputArg);

        // Flags
        var incOption = new Option<bool>("--incremental", "--inc")
        {
            Description = "Enable incremental bit blasting"
        };
        root.Options.Add(incOption);

        var verboseOption = new Option<bool>("--verbose", "-v")
        {
            Description = "Show detailed output"
        };
        root.Options.Add(verboseOption);

        var overflowOption = new Option<bool>("--overflow", "-o")
        {
            Description = "If the additional length of the addition should be considered overflow."
        };
        root.Options.Add(overflowOption);

        var defaultSize = new Option<int>("--defaultSize", "-s")
        {
            Description = "The default size of the constants and identifiers.",
            DefaultValueFactory = _ => 8
        };
        root.Options.Add(defaultSize);

        var valueCapOption = new Option<int>("--valueCap", "-c")
        {
            Description = "The cap how many values should be printed.",
            DefaultValueFactory = _ => 10
        };
        root.Options.Add(valueCapOption);

        var smtLibOutputOption = new Option<string>("--smtLibOutput")
        {
            Description = "The file that should be used for the SMT-Lib code that the implementation generated.",
            DefaultValueFactory = _ => ""
        };
        root.Options.Add(smtLibOutputOption);

        // Bind the handler
        root.SetAction((parseResult) =>
        {
            var opts = new CliOptions
            {
                Input = parseResult.GetValue(inputArg)!,
                Incremental = parseResult.GetValue(incOption),
                Verbose = parseResult.GetValue(verboseOption),
                Overflow = parseResult.GetValue(overflowOption),
                DefaultSize = parseResult.GetValue(defaultSize),
                ValueCap = parseResult.GetValue(valueCapOption),
                SMTLibOutput = Path.GetFullPath(parseResult.GetValue(smtLibOutputOption) ?? throw new UnreachableException()),
            };

            CliHandler.Execute(opts);
        });

        return root;
    }
}
