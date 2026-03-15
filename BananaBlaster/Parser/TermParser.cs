using System.Diagnostics;
using BananaBlaster.Formula;
using BananaBlaster.Formula.Elements;

namespace BananaBlaster.Parser;

class TermParser {
    public static Term? Parse(FormulaLexer lexer)
    {
        var clone = lexer.Clone();
        var term = ParseTermConst(lexer);

        if(term == null)
        {
            lexer.CopyStateFrom(clone);
            term = ParseIdentifier(lexer);
        }

        if(term == null)
        {
            lexer.CopyStateFrom(clone);
            term = ParseParens(lexer);
        }

        if(term == null)
        {
            lexer.CopyStateFrom(clone);
            term = ParseNot(lexer);
        }

        if(term == null) return null;

        // TODO: Extraction and Expansion in here

        // TODO: ITE

        switch(lexer.PeekToken().TokenType)
        {
            case TokenType.AND:
            case TokenType.OR:
            case TokenType.DOT: // Concatenation
            case TokenType.LEFT_SHIFT:
            case TokenType.RIGHT_SHIFT:
            // Addition and Multiplication
            case TokenType.PLUS:
            case TokenType.MINUS:
            case TokenType.STAR:
                break;
            default:
                return term;
        }

        var token = lexer.GetNext();

        var right = Parse(lexer) ?? throw new Exception("The right side of a binary operation cannot be empty.");
        var isConst = right is TermConst;

        return token.TokenType switch {
            TokenType.AND => new TermAnd(term, right),
            TokenType.OR => new TermOr(term, right),
            TokenType.DOT => new TermConcat(term, right),
            TokenType.LEFT_SHIFT => isConst ? new TermLShiftConst(term, IntFromConst((TermConst)right)) : new TermLShift(term, right),
            TokenType.RIGHT_SHIFT => isConst ? new TermRShiftConst(term, IntFromConst((TermConst)right)) : new TermRShift(term, right),

            TokenType.PLUS => new TermSum(term, right),
            TokenType.MINUS => new TermSubtraction(term, right),
            TokenType.STAR => new TermMultiplication(term, right),

            _ => throw new UnreachableException(),
        };
    }

    // Source - https://stackoverflow.com/a/5283199
    // Posted by Luca Fagioli, modified by community. See post 'Timeline' for change history
    // License - CC BY-SA 3.0
    private static int IntFromConst(TermConst termConst)
    {
        if (termConst.Bits.Length > 32)
            throw new ArgumentException("Argument length shall be at most 32 bits.");

        int[] array = new int[1];
        termConst.Bits.CopyTo(array, 0);
        return array[0];
    }

    private static Term? ParseParens(FormulaLexer lexer)
    {
        if(lexer.GetNext().TokenType != TokenType.PAREN_LEFT)
        {
            return null;
        }

        var term = Parse(lexer);

        if(lexer.GetNext().TokenType != TokenType.PAREN_RIGHT)
        {
            return null;
        }

        return term;    
    }

    private static Term? ParseNot(FormulaLexer lexer)
    {
        if(lexer.GetNext().TokenType != TokenType.TILDE) return null;

        var term = Parse(lexer);
        if(term is null) return null;

        return new TermNot(term);
    }

    private static Term? ParseTermConst(FormulaLexer lexer)
    {
        var token = lexer.GetNext();
        if(token.TokenType != TokenType.NUMBER) return null;

        // TODO: Change to customizable length
        return TermConst.From(long.Parse(token.Value ?? throw new UnreachableException()), 8);
    }

    private static Term? ParseIdentifier(FormulaLexer lexer)
    {
        var token = lexer.GetNext();
        if(token.TokenType != TokenType.IDENTIFIER) return null;

        // TODO: Change to customizable length
        return new TermIdentifier(token.Value ?? throw new UnreachableException(), 8);
    }
}
