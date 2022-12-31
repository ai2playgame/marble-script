using System.Text;
using Marble.Processor.AST.Statements;

namespace Marble.Processor.AST.Expression;

// if式
// if文ではないので、値を返す
// if式の例:
//   let max = if (x > y) { x } else { y };
//   if (<condition>) <consequence> else <alternative>
public class IfExpression : IExpression
{
    public Token Token { get; set; }
    public IExpression Condition { get; set; }
    public BlockStatement Consequence { get; set; }
    public BlockStatement? Alternative { get; set; }

    public string ToCode()
    {
        var builder = new StringBuilder();
        builder.Append("if");
        builder.Append(Condition.ToCode());
        builder.Append(' ');
        builder.Append(Consequence.ToCode());

        if (Alternative != null)
        {
            builder.Append("else ");
            builder.Append(Alternative.ToCode());
        }

        return builder.ToString();
    }

    public string TokenLiteral() => Token.Literal;
}