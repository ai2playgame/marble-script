using Marble.Processor;
using Marble.Processor.AST;
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

    [Test]
    public void TestReadPrefixExpression()
    {
        var tests = new[]
        {
            ("!5", "!", 5),
            ("-15", "-", 15),
        };

        foreach (var (input, op, value) in tests)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();

            // エラーがないか確認する
            if (parser.Errors.Count != 0)
            {
                Assert.Fail("エラーが検出されました。");
                return;
            }

            // 文の数は1つ
            Assert.That(root.Statements.Count, Is.EqualTo(1));

            var statement = root.Statements[0] as ExpressionStatement;
            if (statement == null)
            {
                Assert.Fail("statementがExpressionStatement(式文)ではない");
                return;
            }

            var expression = statement.Expression as PrefixExpression;
            if (expression == null)
            {
                Assert.Fail("expressionがPrefixExpression（前置式）ではない");
                return;
            }

            Assert.That(expression.Operator, Is.EqualTo(op));

            _TestIntegerLiteral(expression.Right, value);
        }
    }

    [Test]
    public void TestReadInfixExpression()
    {
        var tests = new (string, object, string, object)[]
        {
            ("1 + 1", 1, "+", 1),
            ("1 - 1", 1, "-", 1),
            ("1 * 1", 1, "*", 1),
            ("1 / 1", 1, "/", 1),
            ("1 < 1", 1, "<", 1),
            ("1 > 1", 1, ">", 1),
            ("1 == 1", 1, "==", 1),
            ("1 != 1", 1, "!=", 1),
            ("true == false", true, "==", false),
            ("true != false", true, "!=", false),
        };

        foreach (var (input, lhs, op, rhs) in tests)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();

            // エラーがないか確認する
            if (parser.Errors.Count != 0)
            {
                Assert.Fail();
                return;
            }

            // 文の数は1つ
            Assert.That(root.Statements.Count, Is.EqualTo(1));

            var statement = root.Statements[0] as ExpressionStatement;
            if (statement == null)
            {
                Assert.Fail("statementがExpressionStatement(式文)ではない");
                return;
            }

            _TestInfixExpression(statement.Expression, lhs, op, rhs);
        }
    }

    public void _TestIntegerLiteral(IExpression expression, int value)
    {
        var integerLiteral = expression as IntegerLiteral;
        if (integerLiteral == null)
        {
            Assert.Fail("Expression が IntegerLiteral ではありません。");
            return;
        }

        if (integerLiteral.Value != value)
        {
            Assert.Fail($"integerLiteral.Value が {value} ではありません。");
        }

        if (integerLiteral.TokenLiteral() != $"{value}")
        {
            Assert.Fail($"ident.TokenLiteral が {value} ではありません。");
        }
    }

    [Test]
    public void TestOperatorPrecedenceParsing()
    {
        var tests = new[]
        {
            ("a + b", "(a + b)"),
            ("!-a", "(!(-a))"),
            ("a + b - c", "((a + b) - c)"),
            ("a * b / c", "((a * b) / c)"),
            ("a + b * c", "(a + (b * c))"),
            ("a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)"),
            ("1 + 2; -3 * 4", "(1 + 2)\r\n((-3) * 4)"),
            ("5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))"),
            ("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"),
            ("true", "true"),
            ("true == false", "(true == false)"),
            ("1 > 2 == false", "((1 > 2) == false)"),
            ("(1 + 2) * 3", "((1 + 2) * 3)"),
            ("1 + (2 - 3)", "(1 + (2 - 3))"),
            ("-(1 + 2)", "(-(1 + 2))"),
            ("!(true == true)", "(!(true == true))"),
            ("1 + (2 - 3) * 4", "(1 + ((2 - 3) * 4))"),
            ("(1 + -(2 + 3)) * 4", "((1 + (-(2 + 3))) * 4)"),
        };

        foreach (var (input, code) in tests)
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

            var actual = root.ToCode();
            Assert.That(actual, Is.EqualTo(code));
        }
    }

    [Test]
    public void TestReadBooleanLiteralExpression()
    {
        var tests = new[]
        {
            ("true;", true),
            ("false;", false),
        };

        foreach (var (input, value) in tests)
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

            Assert.That(root.Statements.Count, Is.EqualTo(1), "文の数は1つでなければならない");

            var statement = root.Statements[0] as ExpressionStatement;
            if (statement == null)
            {
                Assert.Fail("statementがExpressionStatementではない");
                return;
            }

            _TestBooleanLiteral(statement.Expression, value);
        }
    }

    private void _TestBooleanLiteral(IExpression expression, bool value)
    {
        var booleanLiteral = expression as BooleanLiteral;
        if (booleanLiteral == null)
        {
            Assert.Fail();
            return;
        }

        Assert.That(booleanLiteral.Value, Is.EqualTo(value));
        Assert.That(booleanLiteral.TokenLiteral(), Is.EqualTo(value.ToString().ToLower()));
    }

    private void _TestIdentifier(IExpression expression, string value)
    {
        var ident = expression as Identifier;
        if (ident == null)
        {
            Assert.Fail("ExpressionがIdentifierではない");
            return;
        }

        Assert.That(ident.Value, Is.EqualTo(value));
        Assert.That(ident.TokenLiteral(), Is.EqualTo(value));
    }

    private void _TestLiteralExpression(IExpression expression, object expected)
    {
        switch (expected)
        {
            case int intValue:
                _TestIntegerLiteral(expression, intValue);
                break;
            case string stringValue:
                _TestIdentifier(expression, stringValue);
                break;
            case bool boolValue:
                _TestBooleanLiteral(expression, boolValue);
                break;
            default:
                Assert.Fail("予期しない型です");
                break;
        }
    }

    private void _TestInfixExpression(IExpression expression, object left, string op, object right)
    {
        var infixExpression = expression as InfixExpression;
        if (infixExpression == null)
        {
            Assert.Fail("expressionがInfixExpressionではない");
            return;
        }

        _TestLiteralExpression(infixExpression.Lhs, left);
        Assert.That(infixExpression.Operator, Is.EqualTo(op));
        _TestLiteralExpression(infixExpression.Rhs, right);
    }

    [Test]
    public void TestReadIfExpression()
    {
        var input = "if (x < y) { x }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var root = parser.ParseProgram();

        // エラーがあるかどうか
        if (parser.Errors.Count != 0)
        {
            var message = '\n' + string.Join('\n', parser.Errors);
            Assert.Fail(message);
        }

        Assert.That(root.Statements.Count, Is.EqualTo(1), "Root.Statementsの数が間違っている");

        var statement = root.Statements[0] as ExpressionStatement;
        if (statement == null)
        {
            Assert.Fail("statementがExpressionStatementではない");
            return;
        }

        var expression = statement.Expression as IfExpression;
        if (expression == null)
        {
            Assert.Fail("expressionがIfExpressionではない");
            return;
        }

        // If条件部分 (x < y)が正しくparseできてるか調べる
        _TestInfixExpression(expression.Condition, "x", "<", "y");

        // consequence部分をテスト
        Assert.That(expression.Consequence.Statements.Count, Is.EqualTo(1));
        var consequence = expression.Consequence.Statements[0] as ExpressionStatement;
        if (consequence == null)
        {
            Assert.Fail("consequenceがExpressionStatementではない");
            return;
        }

        _TestIdentifier(consequence.Expression, "x");

        // else部分は存在しない
        Assert.That(expression.Alternative, Is.Null);
    }

    [Test]
    public void TestReadIfElseExpression()
    {
        var input = "if (x < y) { x } else { y; }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var root = parser.ParseProgram();

        // エラーがあるかどうか
        if (parser.Errors.Count != 0)
        {
            var message = '\n' + string.Join('\n', parser.Errors);
            Assert.Fail(message);
        }

        Assert.That(root.Statements.Count, Is.EqualTo(1), "Root.Statementsの数が間違っている");

        var statement = root.Statements[0] as ExpressionStatement;
        if (statement == null)
        {
            Assert.Fail("statementがExpressionStatementではない");
            return;
        }

        var expression = statement.Expression as IfExpression;
        if (expression == null)
        {
            Assert.Fail("expressionがIfExpressionではない");
            return;
        }

        // If条件部分 (x < y)が正しくparseできてるか調べる
        _TestInfixExpression(expression.Condition, "x", "<", "y");

        // consequence部分をテスト
        Assert.That(expression.Consequence.Statements.Count, Is.EqualTo(1));
        var consequence = expression.Consequence.Statements[0] as ExpressionStatement;
        if (consequence == null)
        {
            Assert.Fail("consequenceがExpressionStatementではない");
            return;
        }

        _TestIdentifier(consequence.Expression, "x");

        // alternative部分をテスト
        if (expression.Alternative == null)
        {
            Assert.Fail("alternative部分がnull");
            return;
        }

        Assert.That(expression.Alternative.Statements.Count, Is.EqualTo(1));
        var alternative = expression.Alternative.Statements[0] as ExpressionStatement;
        if (alternative == null)
        {
            Assert.Fail("alternativeがExpressionStatementではない");
            return;
        }

        _TestIdentifier(alternative.Expression, "y");
    }
}