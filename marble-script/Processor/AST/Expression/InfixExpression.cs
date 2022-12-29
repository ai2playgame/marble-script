using System.Text;

namespace Marble.Processor.AST.Expression;

// 中置演算子
public class InfixExpression : IExpression
{
    public Token Token { get; set; }
    public IExpression Lhs { get; set; }
    public string Operator { get; set; }
    public IExpression Rhs { get; set; }

    public string ToCode()
    {
        var builder = new StringBuilder();
        builder.Append('(');
        builder.Append(Lhs.ToCode());
        builder.Append(' ');
        builder.Append(Operator);
        builder.Append(' ');
        builder.Append(Rhs.ToCode());
        builder.Append(')');
        return builder.ToString();
    }

    public string TokenLiteral() => Token.Literal;
}