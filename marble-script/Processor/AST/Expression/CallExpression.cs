using System.Text;

namespace Marble.Processor.AST.Expression;

public class CallExpression : IExpression
{
    public Token Token { get; set; }
    public IExpression Function { get; set; }
    public List<IExpression> Arguments { get; set; }

    public string ToCode()
    {
        var args = Arguments.Select(a => a.ToCode());
        var builder = new StringBuilder();
        builder.Append(Function.ToCode());
        builder.Append('(');
        builder.Append(string.Join(", ", args));
        builder.Append(')');
        return builder.ToString();
    }

    public string TokenLiteral() => Token.Literal;
}