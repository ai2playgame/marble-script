namespace Marble.Lexing;

public class Lexer
{
    public string Input { get; private set; }
    public char CurrentChar { get; private set; }
    public char NextChar { get; private set; }
    public int Position { get; private set; } = 0;
    
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
            _ => new Token(TokenType.ILLEGAL, ""),
        };
        
        ReadChar(); // 1文字読み進める
        return token;
    }

    private void ReadChar()
    {
        if (Position >= Input.Length)
        {
            CurrentChar = '\0';
        }
        else
        {
            CurrentChar = Input[Position];
        }

        if (Position + 1 >= Input.Length)
        {
            NextChar = '\0';
        }
        else
        {
            NextChar = Input[Position + 1];
        }

        Position += 1;
    }
}