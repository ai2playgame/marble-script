namespace Marble.Processor.AST.Expression;

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
}