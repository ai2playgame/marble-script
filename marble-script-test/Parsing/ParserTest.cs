using Marble.Processor;
using Marble.Processor.AST;
using Marble.Processor.AST.Statements;
using Marble.Processor.Parsing;

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
let xyz = 838383;";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        Root root = parser.ParseProgram();
        
        Assert.That(root.Statements.Count, Is.EqualTo(3));

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
}