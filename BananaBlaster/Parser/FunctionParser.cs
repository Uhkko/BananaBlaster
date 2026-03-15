using System.Diagnostics;
using BananaBlaster.Formula.Elements;
using BananaBlaster.Parser;
namespace BananaBlaster.Formula;

struct FunctionParser
{
    public static Function? Parse(FormulaLexer lexer)
    {
        var clone = lexer.Clone();

        var function = ParseParens(lexer);
        
        if(function is null)
        {
            lexer.CopyStateFrom(clone);
            function = ParseFuncNot(lexer);
        }

        if(function is null)
        {
            lexer.CopyStateFrom(clone);
            function = ParseAtom(lexer);
        }

        if(function is null) {
            return null;
        }

        switch(lexer.PeekToken().TokenType)
        {
            case TokenType.AND:
            case TokenType.OR:
            case TokenType.EQUIVALENT:
            case TokenType.IMPLIES:
                break;
            default:
                return function;
        }

        var token = lexer.GetNext();

        var right = Parse(lexer) ?? throw new Exception("The right side of a binary operation cannot be empty.");

        return token.TokenType switch {
            TokenType.AND => new FuncAnd(function, right),
            TokenType.OR => new FuncOr(function, right),
            TokenType.EQUIVALENT => new FuncEqual(function, right),
            TokenType.IMPLIES => new FuncImplication(function, right),

            _ => throw new UnreachableException(),
        };
    }

    private static Function? ParseParens(FormulaLexer lexer)
    {
        if(lexer.GetNext().TokenType != TokenType.PAREN_LEFT)
        {
            return null;
        }

        var function = Parse(lexer);

        if(lexer.GetNext().TokenType != TokenType.PAREN_RIGHT)
        {
            return null;
        }

        return function;
    }

    private static Function? ParseFuncNot(FormulaLexer lexer)
    {
        if(lexer.GetNext().TokenType != TokenType.BANG)
        {
            return null;
        }

        var function = Parse(lexer);

        if(function is null) return null;

        return new FuncNot(function);
    }

    private static Function? ParseAtom(FormulaLexer lexer) {
        var atom = AtomParser.Parse(lexer);

        if(atom is null) return null;
        
        return new FuncAtom(atom);
    }
}
