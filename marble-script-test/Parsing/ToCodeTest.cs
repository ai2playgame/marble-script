using Marble.Processor;
using Marble.Processor.AST;
using Marble.Processor.AST.Expression;
using Marble.Processor.AST.Statements;

namespace Marble.Test.Parsing;

public class ToCodeTest
{
    [Test]
    public void TestNodeToCode()
    {
        var code = "let x = abc;";
        var root = new Root();
        root.Statements = new List<IStatement>();
        root.Statements.Add(
            new LetStatement()
            {
                Token = new Token(TokenType.LET, "let"),
                Name = new Identifier(new Token(TokenType.IDENT, "x"), "x"),
                Value = new Identifier(new Token(TokenType.IDENT, "abc"), "abc")
            }
        );
        
        Assert.That(root.ToCode(), Is.EqualTo(code));
    }
}