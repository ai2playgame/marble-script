namespace Marble.Processor.AST;

// ルートノードは、実行するプログラムを子要素として保持する。
// Marbleはlet文（let a = 1 + 1;) のような、文の集まりで構成される。
// 式は値が必要になる箇所、例えばlet文の右辺に登場する。
// 従って、ルートには文がくる
public class Root : INode
{
    public List<IStatement> Statements { get; set; }

    public string TokenLiteral()
    {
        return Statements.FirstOrDefault()?.TokenLiteral() ?? "";
    }
}