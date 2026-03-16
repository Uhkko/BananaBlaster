using System.Diagnostics;
using BananaBlaster.Formula;
using BananaBlaster.Formula.Elements;

namespace BananaBlaster.Parser;

class TermParser {
    public static Term? Parse(FormulaLexer lexer, ParsingContext context)
    {
        var clone = lexer.Clone();
        var term = ParseTermConst(lexer, context);

        if(term == null)
        {
            lexer.CopyStateFrom(clone);
            term = ParseIdentifier(lexer, context);
        }

        if(term == null)
        {
            lexer.CopyStateFrom(clone);
            term = ParseParens(lexer, context);
        }

        if(term == null)
        {
            lexer.CopyStateFrom(clone);
            term = ParseNot(lexer, context);
        }

        if(term == null) return null;

        clone = lexer.Clone();
        var extractedTerm = ParseExtraction(lexer, term);
        if(extractedTerm != null)
            term = extractedTerm;
        else
            lexer.CopyStateFrom(clone);

        // TODO: Expansion

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

        var right = Parse(lexer, context) ?? throw new Exception("The right side of a binary operation cannot be empty.");
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

    private static Term? ParseParens(FormulaLexer lexer, ParsingContext context)
    {
        if(lexer.GetNext().TokenType != TokenType.PAREN_LEFT)
        {
            return null;
        }

        var term = Parse(lexer, context);

        if(lexer.GetNext().TokenType != TokenType.PAREN_RIGHT)
        {
            return null;
        }

        return term;    
    }

    private static Term? ParseNot(FormulaLexer lexer, ParsingContext context)
    {
        if(lexer.GetNext().TokenType != TokenType.TILDE) return null;

        var term = Parse(lexer, context);
        if(term is null) return null;

        return new TermNot(term);
    }

    private static Term? ParseTermConst(FormulaLexer lexer, ParsingContext context)
    {
        var token = lexer.GetNext();
        if(token.TokenType != TokenType.NUMBER) return null;

        var size = ParseSize(lexer, context.DefaultSize);

        return TermConst.From(long.Parse(token.Value ?? throw new UnreachableException()), size);
    }

    private static Term? ParseIdentifier(FormulaLexer lexer, ParsingContext context)
    {
        var token = lexer.GetNext();
        if(token.TokenType != TokenType.IDENTIFIER) return null;

        var size = ParseSize(lexer, context.DefaultSize);

        return new TermIdentifier(token.Value ?? throw new UnreachableException(), size);
    }

    private static int ParseSize(FormulaLexer lexer, int defaultSize)
    {
        var clone = lexer.Clone();

        if(lexer.GetNext().TokenType != TokenType.UNDERSCORE)
        {
            lexer.CopyStateFrom(clone);
            return defaultSize;
        }

        var number = lexer.GetNext();
        if(number.TokenType != TokenType.NUMBER)
        {
            lexer.CopyStateFrom(clone);
            return defaultSize; // Should probably error out
        }

        return int.Parse(number.Value ?? throw new UnreachableException());
    }

    private static Term? ParseExtraction(FormulaLexer lexer, Term term)
    {
        if(lexer.GetNext().TokenType != TokenType.BRACKET_LEFT) return null;

        var from = lexer.GetNext();
        if(from.TokenType != TokenType.NUMBER) return null;

        if(lexer.GetNext().TokenType != TokenType.DOT_DOT) return null;

        var to = lexer.GetNext();
        if(to.TokenType != TokenType.NUMBER) return null;

        if(lexer.GetNext().TokenType != TokenType.BRACKET_RIGHT) return null;

        // TODO: Check if these values actually make sense
        return new TermExtraction(
            term,
            int.Parse(from.Value ?? throw new UnreachableException()) - 1, // One index
            int.Parse(to.Value ?? throw new UnreachableException()) - 1    // One index
        );
    }
}
