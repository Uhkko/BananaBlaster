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

        // Bind the handler
        root.SetAction((parseResult) =>
        {
            var opts = new CliOptions
            {
                Input = parseResult.GetValue(inputArg)!,
                Incremental = parseResult.GetValue(incOption),
                Verbose = parseResult.GetValue(verboseOption),
            };

            CliHandler.Execute(opts);
        });

        return root;
    }
}
