namespace BananaBlaster.Parser;

class FormulaLexer(string content)
{
    private readonly string content = content;
    private int position = 0;          // index in the source string

    public Token GetNext()
    {
        if (position >= content.Length)
            return new Token(TokenType.EOL, position);

        while (position < content.Length && char.IsWhiteSpace(content[position]))
            position++;

        if (position >= content.Length)
            return new Token(TokenType.EOL, position);

        char ch = content[position];

        TokenType? tokenType = ch switch
        {
            '(' => TokenType.PAREN_LEFT,
            ')' => TokenType.PAREN_RIGHT,
            '[' => TokenType.BRACKET_LEFT,
            ']' => TokenType.BRACKET_RIGHT,
            '<' when peek() == '=' && peek(2) == '>' && ((position += 2) > 0) => TokenType.EQUIVALENT,
            '=' when peek() == '>' && (++position > 0) => TokenType.IMPLIES,
            '=' => TokenType.EQUALS,
            '<' when peek() == '<' && (++position > 0) => TokenType.LEFT_SHIFT,
            '<' => TokenType.LESS_THAN,
            '>' when peek() == '>' && (++position > 0) => TokenType.RIGHT_SHIFT,
            '>' => TokenType.GREATER_THAN,
            '+' => TokenType.PLUS,
            '-' => TokenType.MINUS,
            '*' => TokenType.STAR,
            '?' => TokenType.QUESTION_MARK,
            ':' => TokenType.COLON,
            '.' when peek() == '.' && (++position > 0) => TokenType.DOT_DOT,
            '.' => TokenType.DOT,
            '&' when peek() == '&' && (++position > 0) => TokenType.AND_AND,
            '&' => TokenType.AND,
            '|' when peek() == '|' && (++position > 0) => TokenType.OR_OR,
            '|' => TokenType.OR,
            '!' => TokenType.BANG,
            '~' => TokenType.TILDE,
            _   => null
        };

        if (tokenType != null)
        {
            position++; // consume the character
            return new Token(tokenType.Value, position - 1);
        }

        if (char.IsDigit(ch))
        {
            int start = position;
            while (position < content.Length && char.IsDigit(content[position]))
                position++;

            string numText = content[start..position];
            return new Token(TokenType.NUMBER, start, numText);
        }

        if (char.IsLetter(ch) || ch == '_')
        {
            int start = position;
            while (position < content.Length &&
                   (char.IsLetterOrDigit(content[position]) || content[position] == '_'))
                position++;

            string idText = content[start..position];

            if (idText == "false") return new Token(TokenType.FALSE, start);
            if (idText == "true")  return new Token(TokenType.TRUE, start);

            return new Token(TokenType.IDENTIFIER, start, idText);
        }

        throw new Exception($"Unexpected character while lexing. Character '{ch}' not known.");
    }

    private char peek(int amount = 1)
    {
        if (content.Length <= (position + amount))
            return '\0';

        return content[position + amount];
    }

    public FormulaLexer Clone()
    {
        return (FormulaLexer)MemberwiseClone();
    }

    public void CopyStateFrom(FormulaLexer source)
    {
        if (!ReferenceEquals(this.content, source.content))
            throw new InvalidOperationException("Cannot copy state from a lexer that scans a different source string.");

        this.position = source.position;
    }

    public Token PeekToken()
    {
        var clone = this.Clone();
        return clone.GetNext();
    }
}

struct Token(TokenType tokenType, int position)
{
    public TokenType TokenType { get; } = tokenType;
    public int Position { get; } = position;
    public string? Value { get; }

    public Token(TokenType tokenType, int position, string value) : this(tokenType, position)
    {
        Value = value;
    }

    public override string ToString()
    {
        return $"Token{{TokenType: {TokenType}, Position: {Position}, Value: {Value} }}";
    }
}

enum TokenType
{
    PAREN_LEFT,
    PAREN_RIGHT,
    BRACKET_LEFT,
    BRACKET_RIGHT,
    LEFT_SHIFT,
    RIGHT_SHIFT,
    QUESTION_MARK,
    COLON,
    DOT, // Concat
    DOT_DOT,
    AND,
    AND_AND,
    OR,
    OR_OR,
    BANG,
    TILDE,
    EQUIVALENT,
    EQUALS,
    IMPLIES,
    LESS_THAN,
    GREATER_THAN,
    PLUS,
    MINUS,
    STAR,
    IDENTIFIER,
    TRUE,
    FALSE,
    NUMBER,
    EOL,
}
