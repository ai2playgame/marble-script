// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Marble.Lexing;

Console.WriteLine("Hello, World!");
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
        Console.WriteLine(token.Type + token.Literal);
    }
}
