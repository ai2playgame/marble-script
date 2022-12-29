namespace Marble.Processor.AST.Expression;

public class PrefixExpression : IExpression
{
    public Token Token { get; set; }
    public string Operator { get; set; }
    public IExpression Right { get; set; }

    public string ToCode() => $"{Operator}{Right.ToCode()}";
    public string TokenLiteral() => Token.Literal;
}