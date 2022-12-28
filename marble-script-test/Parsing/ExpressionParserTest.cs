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
        if (parser.Errors.Count != 0)
        {
            Assert.Fail("エラーが検出されました。");
            return;
        }
        
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

    [Test]
    public void TestReadIntegerLiteralExpression()
    {
        var input = @"123;";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var root = parser.ParseProgram();
        
        // エラーがないか確認する
        if (parser.Errors.Count != 0)
        {
            Assert.Fail("エラーが検出されました。");
            return;
        }

        Assert.That(root.Statements.Count, Is.EqualTo(1), "Root.Statementsの数が間違っています。");

        var statement = root.Statements[0] as ExpressionStatement;
        if (statement == null)
        {
            Assert.Fail("statementがExpressionStatementではない");
            return;
        }

        var integerLiteral = statement.Expression as IntegerLiteral;
        if (integerLiteral == null)
        {
            Assert.Fail("ExpressionがIntegerLiteralではない");
            return;
        }
        
        Assert.That(integerLiteral.Value, Is.EqualTo(123));
        Assert.That(integerLiteral.TokenLiteral(), Is.EqualTo("123"));
    }
}