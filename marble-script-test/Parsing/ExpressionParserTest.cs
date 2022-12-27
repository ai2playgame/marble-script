using Marble.Processor;
using Marble.Processor.AST.Expression;
using Marble.Processor.AST.Statements;
using Marble.Processor.Parsing;

namespace Marble.Test.Parsing;

public class ExpressionParserTest
{
    [Test]
    public void TestReadIdentifierExpression()
    {
        var input = @"foobar;";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var root = parser.ParseProgram();
        
        // エラーは検出されていないか？
        Assert.That(parser.Errors.Count, Is.EqualTo(0));
        
        // 評価される式の数は一つ
        Assert.That(root.Statements.Count, Is.EqualTo(1));

        var statement = root.Statements[0] as ExpressionStatement;
        if (statement == null)
        {
            Assert.Fail("statementがExpressionStatementではない");
            return;
        }

        // 識別子foobarを読み取れているか？
        var ident = statement.Expression as Identifier;
        if (ident == null)
        {
            Assert.Fail("ExpressionがIdentifierではない");
            return;
        }
        Assert.That(ident.Value, Is.EqualTo("foobar"));
        Assert.That(ident.TokenLiteral(), Is.EqualTo("foobar"));
    }
}