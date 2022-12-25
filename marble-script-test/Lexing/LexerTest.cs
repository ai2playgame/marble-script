using Marble.Lexing;
using NuGet.Frameworks;

namespace Marble.Test.Lexing;

public class LexerTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestNextToken1()
    {
        var input = "=+(){},;";
        
        var testTokens = new List<Token>();
        testTokens.Add(new Token(TokenType.ASSIGN, "="));
        testTokens.Add(new Token(TokenType.PLUS, "+"));
        testTokens.Add(new Token(TokenType.LPAREN, "("));
        testTokens.Add(new Token(TokenType.RPAREN, ")"));
        testTokens.Add(new Token(TokenType.LBRACE, "{"));
        testTokens.Add(new Token(TokenType.RBRACE, "}"));
        testTokens.Add(new Token(TokenType.COMMA, ","));
        testTokens.Add(new Token(TokenType.SEMICOLON, ";"));
        testTokens.Add(new Token(TokenType.EOF, ""));

        var lexer = new Lexer(input);

        foreach (var testToken in testTokens)
        {
            var token = lexer.NextToken();
            Assert.That(token.Type, Is.EqualTo(testToken.Type));
            Assert.That(token.Literal, Is.EqualTo(testToken.Literal));
        }
    }
}
