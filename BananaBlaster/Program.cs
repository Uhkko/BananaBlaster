using BananaBlaster.Cli;

namespace BananaBlaster;

public static class Program
{
    public static int Main(string[] args)
    {
        return CliBuilder.BuildRootCommand().Parse(args).Invoke();
    }
}