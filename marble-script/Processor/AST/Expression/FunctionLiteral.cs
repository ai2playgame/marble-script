using System.Text;
using Marble.Processor.AST.Statements;

namespace Marble.Processor.AST.Expression;

// 関数リテラル
// fn <parameter> <block statement>
// e.g.
// fn(x, y) { return x + y; }
public class FunctionLiteral : IExpression
{
    public Token Token { get; set; }
    public List<Identifier> Parameters { get; set; }
    public BlockStatement Body { get; set; }

    public string ToCode()
    {
        var builder = new StringBuilder();
        builder.Append("fn");
        builder.Append('(');
        var parameters = Parameters.Select(p => p.ToCode());
        builder.Append(string.Join(", ", parameters));
        builder.Append(')');
        builder.Append(Body.ToCode());
        return builder.ToString();
    }

    public string TokenLiteral() => Token.Literal;
}