namespace Marble.Processor.AST.Statements;

// 式文
// 例) 1 + 1;
// 式だけでなる文のこと。C#では認められていない文法
public class ExpressionStatement : IStatement
{
    // 式の最初に出現するトークン
    public Token HeadToken { get; set; }
    public IExpression? Expression { get; set; }
    
    public string TokenLiteral()
    {
        return HeadToken.Literal;
    }

    public string ToCode()
    {
        return Expression?.ToCode() ?? "";
    }
}