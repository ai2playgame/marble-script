namespace Marble.Lexing;

public class Lexer
{
    private string Input { get; set; }
    private char CurrentChar { get; set; }
    private char NextChar { get; set; }
    private int Position { get; set; }
    
    public Lexer(string input)
    {
        Input = input;
        ReadChar(); // 1文字だけ読み進める
    }

    public Token NextToken()
    {
        Token token = CurrentChar switch
        {
            '=' => new Token(TokenType.ASSIGN, CurrentChar.ToString()),
            '+' => new Token(TokenType.PLUS, CurrentChar.ToString()),
            '(' => new Token(TokenType.LPAREN, CurrentChar.ToString()),
            ')' => new Token(TokenType.RPAREN, CurrentChar.ToString()),
            '{' => new Token(TokenType.LBRACE, CurrentChar.ToString()),
            '}' => new Token(TokenType.RBRACE, CurrentChar.ToString()),
            ',' => new Token(TokenType.COMMA, CurrentChar.ToString()),
            ';' => new Token(TokenType.SEMICOLON, CurrentChar.ToString()),
            '\0' => new Token(TokenType.EOF, ""),
            _ => ReadDefaultPatternToken()
        };
        
        ReadChar(); // 1文字読み進める
        return token;
    }

    private void ReadChar()
    {
        CurrentChar = (Position < Input.Length) ? Input[Position] : '\0';

        NextChar = (Position + 1 < Input.Length) ? Input[Position + 1] : '\0';

        Position += 1;
    }

    private Token ReadDefaultPatternToken()
    {
        if (IsLetter(CurrentChar))
        {
            var identifier = ReadIdentifier();
            var type = LookupIdentifier(identifier);
            return new Token(type, identifier);
        }

        return new Token(TokenType.ILLEGAL, CurrentChar.ToString());
    }

    private string ReadIdentifier()
    {
        var identifier = CurrentChar.ToString();
        
        // 次の文字がLetterであればそれを読み取って加える
        while (IsLetter(NextChar))
        {
            identifier += NextChar;
            ReadChar();
        }

        return identifier;
    }

    private bool IsLetter(char c)
    {
        return c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or ('_');
    }
    
    // 渡された識別子に対応するTokenTypeを返す
    // ex: identifier = "let"の場合、予約語であるためTokenType.LETを返す
    private static TokenType LookupIdentifier(string identifier)
    {
        return Keywords.ContainsKey(identifier) ?
            Keywords[identifier] : TokenType.IDENT;
    }

    private static readonly Dictionary<string, TokenType> Keywords
        = new()
        {
            { "let", TokenType.LET },
            { "fn", TokenType.FUNCTION },
        };
}