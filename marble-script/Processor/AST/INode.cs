namespace Marble.Processor.AST;

public interface INode
{
    string TokenLiteral();
    string ToCode(); // デバッグ用。各ノードの状態に対応したコードを返す
}