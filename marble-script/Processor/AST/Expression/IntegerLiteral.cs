using System.Reflection.Metadata.Ecma335;

namespace Marble.Processor.AST.Expression;

public class IntegerLiteral : IExpression
{
    public Token Token { get; set; }
    public int Value { get; set; }

    public string ToCode() => Token.Literal;
    public string TokenLiteral() => Token.Literal;
}
