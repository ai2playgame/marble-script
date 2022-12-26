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
        SkipWhiteSpace(); // 空白は読み飛ばす仕様
        
        Token token = CurrentChar switch
        {
            '=' => StartWithEqualToken(),
            '+' => new Token(TokenType.PLUS, CurrentChar.ToString()),
            '-' => new Token(TokenType.MINUS, CurrentChar.ToString()),
            '*' => new Token(TokenType.ASTERISK, CurrentChar.ToString()),
            '/' => new Token(TokenType.SLASH, CurrentChar.ToString()),
            '!' => StartWithBangToken(),
            '>' => new Token(TokenType.GT, CurrentChar.ToString()),
            '<' => new Token(TokenType.LT, CurrentChar.ToString()),
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

    // =から始まるトークンを処理する
    private Token StartWithEqualToken()
    {
        if (this.NextChar == '=')
        {
            // ==演算子
            this.ReadChar(); // 1文字読み進める
            return new Token(TokenType.EQ, "==");
        }
        else
        {
            return new Token(TokenType.ASSIGN, CurrentChar.ToString());
        }
    }

    // !から始まるTokenを処理する
    private Token StartWithBangToken()
    {
        if (this.NextChar == '=')
        {
            // !=演算子
            this.ReadChar(); // 1文字読み進める
            return new Token(TokenType.NOT_EQ, "!=");
        }
        else
        {
            return new Token(TokenType.BANG, CurrentChar.ToString());
        }
    }
    
    private void SkipWhiteSpace()
    {
        // 半角スペース,タブ文字,改行文字を読み飛ばす
        while (CurrentChar is ' ' or '\t' or '\r' or '\n')
        {
            ReadChar();
        }
    }

    private Token ReadDefaultPatternToken()
    {
        if (IsLetter(CurrentChar))
        {
            var identifier = ReadIdentifier();
            var type = LookupIdentifier(identifier);
            return new Token(type, identifier);
        }
        else if (IsDigit(CurrentChar))
        {
            var number = ReadNumber();
            return new Token(TokenType.INT, number);
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

    private string ReadNumber()
    {
        var number = CurrentChar.ToString();
        
        // 次の文字が数字であれば引き続きそれを読み取って加える
        while (IsDigit(NextChar))
        {
            number += NextChar;
            ReadChar();
        }

        return number;
    }

    private bool IsLetter(char c)
    {
        return c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or ('_');
    }

    private bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
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
            { "fn", TokenType.FUNCTION },
            { "let", TokenType.LET },
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "return", TokenType.RETURN },
            { "true", TokenType.TRUE },
            { "false", TokenType.FALSE },
        };
}