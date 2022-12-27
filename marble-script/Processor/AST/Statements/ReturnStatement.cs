using System.Text;

namespace Marble.Processor.AST.Statements;

// return文
// return <expression (式)>;
public class ReturnStatement : IStatement
{
    public Token Token { get; set; }
    public IExpression ReturnValue { get; set; }

    public string TokenLiteral() => Token.Literal;
    public string ToCode()
    {
        StringBuilder builder = new();
        builder.Append(Token?.Literal ?? "");
        builder.Append(" ");
        builder.Append(ReturnValue?.ToCode() ?? "");
        builder.Append(";");
        return builder.ToString();
    }
}