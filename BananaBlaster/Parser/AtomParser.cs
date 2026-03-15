using System.Diagnostics;
using BananaBlaster.Formula;
using BananaBlaster.Formula.Elements;

namespace BananaBlaster.Parser;

class AtomParser {
    public static Atom? Parse(FormulaLexer lexer)
    {
        var clone = lexer.Clone();

        var atom = ParseConst(lexer);

        if(atom is null)
        {
            lexer.CopyStateFrom(clone);
            atom = ParseIdentifier(lexer);
        }

        if(atom != null) return atom;

        lexer.CopyStateFrom(clone);
        var term = TermParser.Parse(lexer);
        if(term == null) return null;

        clone = lexer.Clone();
        atom = ParseExtraction(lexer, term);
        if(atom != null) return atom;

        lexer.CopyStateFrom(clone);
        
        var token = lexer.GetNext();

        var right = TermParser.Parse(lexer) ?? throw new Exception("The right side of a binary operation cannot be empty.");

        return token.TokenType switch {
            TokenType.EQUALS => new AtomEquals(term, right),
            TokenType.LESS_THAN => new AtomLess(term, right),
            TokenType.GREATER_THAN => new AtomGreater(term, right),

            _ => null,
        };
    }

    private static Atom? ParseConst(FormulaLexer lexer)
    {
        var token = lexer.GetNext();

        if(token.TokenType == TokenType.FALSE) return new AtomConst(false);
        if(token.TokenType == TokenType.TRUE) return new AtomConst(true);

        return null;
    }

    private static Atom? ParseIdentifier(FormulaLexer lexer)
    {
        var token = lexer.GetNext();

        if(token.TokenType != TokenType.IDENTIFIER) return null;

        return new AtomIdentifier(token.Value ?? throw new UnreachableException());
    }

    private static Atom? ParseExtraction(FormulaLexer lexer, Term term)
    {
        // term[number]
        if(lexer.GetNext().TokenType != TokenType.BRACKET_LEFT) return null;
        var number = lexer.GetNext();
        
        if(number.TokenType != TokenType.NUMBER) return null;
        if(lexer.GetNext().TokenType != TokenType.BRACKET_RIGHT) return null;

        return new AtomExtraction(term, int.Parse(number.Value ?? throw new UnreachableException()));
    }
}
