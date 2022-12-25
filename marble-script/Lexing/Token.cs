namespace Marble.Lexing;

public record Token(TokenType Type, string Literal);
public enum TokenType
{
    ILLEGAL, // 不正なトークン
    EOF,     // 終端

    IDENT, // 識別子
    INT,   // 整数リテラル

    ASSIGN, // =演算子
    PLUS,   // +演算子

    COMMA,     // デリミタ（コンマ）
    SEMICOLON, // デリミタ（セミコロン）

    LPAREN, // (
    RPAREN, // )
    LBRACE, // {
    RBRACE, // }

    FUNCTION, // キーワード
    LET,      // キーワード
}
