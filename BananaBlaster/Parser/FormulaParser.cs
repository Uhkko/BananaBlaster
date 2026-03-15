using BananaBlaster.Formula;

namespace BananaBlaster.Parser;

public class FormulaParser {
    public static Function Parse(string content) {
        var lexer = new FormulaLexer(content);

        return FunctionParser.Parse(lexer) ?? throw new Exception("Could not parse the formula.");
    }
}

