namespace Marble.Processor;

public record Token(TokenType Type, string Literal);
public enum TokenType
{
    ILLEGAL, // 不正なトークン
    EOF,     // 終端

    IDENT, // 識別子
    INT,   // 整数リテラル

    ASSIGN,   // =演算子
    PLUS,     // +演算子
    MINUS,    // -演算子
    ASTERISK, // *演算子
    SLASH,    // /演算子
    BANG,     // !演算子
    GT,       // >演算子
    LT,       // <演算子
    EQ,       // ==演算子
    NOT_EQ,   // !=演算子

    COMMA,     // デリミタ（コンマ）
    SEMICOLON, // デリミタ（セミコロン）

    LPAREN, // (
    RPAREN, // )
    LBRACE, // {
    RBRACE, // }

    FUNCTION, // 関数定義キーワード
    LET,      // 変数定義キーワード
    IF,       // if文
    ELSE,     // if文
    RETURN,   // returnキーワード

    TRUE,  // true
    FALSE, // false
}
