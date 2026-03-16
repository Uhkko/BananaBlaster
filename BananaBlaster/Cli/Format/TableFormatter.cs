using System.Text;

namespace BananaBlaster.Cli.Format;

public static class TableFormatter
{
    public static string FormatTable<TKey, TValue>(
        IDictionary<TKey, TValue> data,
        int cap = 20
    )
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (cap < 0) throw new ArgumentOutOfRangeException(nameof(cap));

        var keyStrings   = data.Keys.Select(k => k?.ToString() ?? "null").ToList();
        var valueStrings = data.Values.Select(v => v?.ToString() ?? "null").ToList();

        // Determine column widths
        int keyWidth = Math.Max("Key".Length, keyStrings.DefaultIfEmpty(string.Empty).Max(s => s.Length));
        int valueWidth = Math.Max(
            Math.Max("Value".Length,
                valueStrings.DefaultIfEmpty(string.Empty).Max(s => s.Length)
            ),
            data.Count > cap ? $"({data.Count - cap} more)".Length : 0
        );

        var sb = new StringBuilder();

        // ┌ ┬ ┐ └ ┴ ┘ ─ │ ├ ┼ ┤  – Unicode box‑drawing characters
        string topBorder    = $"┌{new string('─', keyWidth + 2)}┬{new string('─', valueWidth + 2)}┐";
        string header       = $"│ {PadRight("Key",   keyWidth)} │ {PadRight("Value", valueWidth)} │";
        string separator    = $"├{new string('─', keyWidth + 2)}┼{new string('─', valueWidth + 2)}┤";
        string bottomBorder = $"└{new string('─', keyWidth + 2)}┴{new string('─', valueWidth + 2)}┘";

        sb.AppendLine(topBorder);
        sb.AppendLine(header);
        sb.AppendLine(separator);

        var rows = data.Take(cap).ToList();
        foreach (var kvp in rows)
        {
            string keyStr   = kvp.Key?.ToString() ?? "null";
            string valueStr = kvp.Value?.ToString() ?? "null";

            string row = $"│ {PadRight(keyStr,   keyWidth)} │ {PadRight(valueStr, valueWidth)} │";
            sb.AppendLine(row);
        }

        if (data.Count > cap)
        {
            string ellipsis = $"│ {PadRight("...", keyWidth)} │ {PadRight($"({data.Count - cap} more)", valueWidth)} │";
            sb.AppendLine(ellipsis);
        }

        sb.AppendLine(bottomBorder);
        return sb.ToString();
    }

    private static string PadRight(string text, int totalWidth) =>
        text.PadRight(totalWidth);
}
