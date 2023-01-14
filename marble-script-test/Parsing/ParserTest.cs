using System.Text;
using Marble.Processor;
using Marble.Processor.AST;
using Marble.Processor.AST.Expression;
using Marble.Processor.AST.Statements;
using Marble.Processor.Parsing;
using NuGet.Frameworks;

namespace Marble.Test.Parsing;

public class ParserTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestReadLetStatement()
    {
        var input = @"let x = 5;
let y = 10;
let xyz = 636363;";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        Root root = parser.ParseProgram();
        
        // エラーがないかを確かめる
        if (parser.Errors.Count != 0)
        {
            StringBuilder errorMessages = new();
            // エラーがあれば、そのメッセージを表示する
            parser.Errors.ForEach(errorMessage =>
            {
                errorMessages.AppendLine(errorMessage);
            });
            Assert.Fail(errorMessages.ToString().TrimEnd());
        }
        
        Assert.That(root.Statements.Count, Is.EqualTo(3), "文の数が間違っています");
        var tests = new string[] { "x", "y", "xyz" };
        for (int i = 0; i < tests.Length; i++)
        {
            var name = tests[i];
            var statement = root.Statements[i];
            _TestReadLetStatement(statement, name);
        }
    }

    private void _TestReadLetStatement(IStatement statement, string name)
    {
        Assert.That(statement.TokenLiteral(), Is.EqualTo("let"),
            "TokenLiteralがletではありません");

        var letStatement = statement as LetStatement;
        if (letStatement == null)
        {
            Assert.Fail("statementがLetStatementではない");
            return;
        }
        
        Assert.That(letStatement.Name.Value, Is.EqualTo(name), "識別子が間違っています");
        Assert.That(letStatement.Name.TokenLiteral(), Is.EqualTo(name), "識別子のリテラルが間違っています");
    }

    [Test]
    public void TestReadReturnStatement1()
    {
        var tests = new (string, object)[]
        {
            ("return 5;", 5),
            ("return true;", true),
            ("return x;", "x"),
        };
        foreach (var (input, expected) in tests)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            
            // エラーがあるかどうか
            if (parser.Errors.Count != 0)
            {
                var message = '\n' + string.Join('\n', parser.Errors);
                Assert.Fail(message);
            }
            
            Assert.That(root.Statements.Count, Is.EqualTo(1));

            var returnStatement = root.Statements[0] as ReturnStatement;
            Assert.That(returnStatement, Is.Not.Null);
            
            Assert.That(returnStatement.TokenLiteral(), Is.EqualTo("return"));
            
            _TestLiteralExpression(returnStatement.ReturnValue, expected);
        }
    }

    private void _TestLiteralExpression(IExpression expression, object expected)
    {
        ExpressionParserTest._TestLiteralExpression(expression, expected);
    }
}