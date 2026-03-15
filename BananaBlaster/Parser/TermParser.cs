using BananaBlaster.Formula;
using BananaBlaster.Formula.Elements;

namespace BananaBlaster.Parser;

class TermParser {
    public static Term? Parse(FormulaLexer lexer) {
        var token = lexer.GetNext();
        if(token.TokenType != TokenType.NUMBER) return null;

        // TODO: Implement TermParser
        return TermConst.From(0b1010, 4);
    }
}
