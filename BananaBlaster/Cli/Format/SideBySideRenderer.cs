namespace BananaBlaster.Cli.Format;

public static class SideBySideRenderer
{
    public static string Render<TKey1, TValue1, TKey2, TValue2>(
        IDictionary<TKey1, TValue1> leftData,
        IDictionary<TKey2, TValue2> rightData,
        string leftTitle,
        string rightTitle,
        int cap = 20,
        int gap = 4
    )
    {
        string leftTable  = TableFormatter.FormatTable(leftData,  cap);
        string rightTable = TableFormatter.FormatTable(rightData, cap);

        // Split into lines
        var leftLines  = leftTable.Split('\n');
        if(leftLines.Last() == "")
            leftLines = leftLines[0..(leftLines.Length - 1)];
        
        var rightLines = rightTable.Split('\n');

        // Title lines – underlined using ANSI escape \x1B[4m … \x1B[0m
        string leftTitleLine  = $"\x1B[4m{leftTitle}\x1B[0m";
        string rightTitleLine = $"\x1B[4m{rightTitle}\x1B[0m";

        var combinedLines = new List<string>
        {
            $" {leftTitleLine}{new string(' ', leftLines[0].Length - leftTitle.Length + gap)}{rightTitleLine}"
        };

        int maxHeight = Math.Max(leftLines.Length, rightLines.Length);
        var leftPadded  = leftLines .Concat(Enumerable.Repeat(new string(' ', leftLines[0].Length), maxHeight - leftLines.Length)).ToArray();
        var rightPadded = rightLines.Concat(Enumerable.Repeat(new string(' ', rightLines[0].Length), maxHeight - rightLines.Length)).ToArray();

        string spacer = new(' ', gap);
        for (int i = 0; i < maxHeight; i++)
        {
            combinedLines.Add($"{leftPadded[i]}{spacer}{rightPadded[i]}");
        }

        return string.Join(Environment.NewLine, combinedLines);
    }
}
