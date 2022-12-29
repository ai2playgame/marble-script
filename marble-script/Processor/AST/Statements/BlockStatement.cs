using System.Text;

namespace Marble.Processor.AST.Statements;

public class BlockStatement : IStatement
{
    public Token Token { get; set; }
    public List<IStatement> Statements { get; set; }

    public string ToCode()
    {
        var builder = new StringBuilder();
        foreach (var statement in Statements)
        {
            builder.Append(statement.ToCode());
        }

        return builder.ToString();
    }

    public string TokenLiteral() => Token.Literal;
}