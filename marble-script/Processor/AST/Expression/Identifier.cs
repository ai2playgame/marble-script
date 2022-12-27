namespace Marble.Processor.AST.Expression;

// 識別子
public class Identifier : IExpression
{
    public Token Token { get; set; }
    public string Value { get; set; }

    public Identifier(Token token, string value)
    {
        Token = token;
        Value = value;
    }

    public string TokenLiteral() => Token.Literal;
    public string ToCode() => Token.Literal;
}