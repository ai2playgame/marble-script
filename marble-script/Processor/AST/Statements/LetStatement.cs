using System.Text;
using Marble.Processor.AST.Expression;

namespace Marble.Processor.AST.Statements;

// let文
// let <identifier> = <expression>;
public class LetStatement : IStatement
{
    public Token Token { get; set; }
    public Identifier Name { get; set; }
    public IExpression Value { get; set; }
    public string TokenLiteral() => Token.Literal;

    public string ToCode()
    {
        StringBuilder builder = new();
        builder.Append(Token?.Literal ?? "");
        builder.Append(" ");
        builder.Append(Name?.ToCode() ?? "");
        builder.Append(" = ");
        builder.Append(Value?.ToCode() ?? "");
        builder.Append(";");
        return builder.ToString();
    }
}