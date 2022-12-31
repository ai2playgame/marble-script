namespace Marble.Processor.AST.Expression;

public class BooleanLiteral : IExpression
{
    public Token Token { get; set; }
    public bool Value { get; set; }

    public string ToCode() => Token.Literal;
    public string TokenLiteral() => Token.Literal;
}