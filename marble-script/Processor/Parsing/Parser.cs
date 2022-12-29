using System.Runtime.InteropServices.JavaScript;
using Marble.Processor.AST;
using Marble.Processor.AST.Expression;
using Marble.Processor.AST.Statements;

namespace Marble.Processor.Parsing
{
    // 前置構文解析関数
    using PrefixParseFn = Func<IExpression>;
    // 中置構文解析関数
    using InfixParseFn = Func<IExpression, IExpression>;

    public class Parser
    {
        public Token CurrentToken { get; set; }
        public Token NextToken { get; set; }
        public Lexer Lexer { get; }
        public List<string> Errors { get; set; }

        public Dictionary<TokenType, PrefixParseFn> PrefixParseFns { get; set; }
        public Dictionary<TokenType, InfixParseFn> InfixParseFns { get; set; }

        public Parser(Lexer lexer)
        {
            // フィールド初期化
            Lexer = lexer;
            Errors = new();

            // トークンの種別と、前置構文解析関数を紐づける
            RegisterPrefixParseFns();
            InfixParseFns = new();

            // 2つ分のトークンを先に読み込んでおく
            CurrentToken = Lexer.NextToken();
            NextToken = Lexer.NextToken();
            Errors = new List<string>();
        }

        // 式をパースして、ASTを構築し、ルートに追加していく
        public Root ParseProgram()
        {
            var root = new Root
            {
                Statements = new List<IStatement>()
            };
            while (CurrentToken.Type != TokenType.EOF)
            {
                var statement = ParseStatement();
                if (statement != null)
                {
                    root.Statements.Add(statement);
                }

                ReadToken();
            }

            return root;
        }

        private void ReadToken()
        {
            CurrentToken = NextToken;
            NextToken = Lexer.NextToken();
        }

        private IStatement? ParseStatement()
        {
            return CurrentToken.Type switch
            {
                TokenType.LET => ParseLetStatement(),
                TokenType.RETURN => ParseReturnStatement(),
                _ => ParseExpressionStatement(), // let,return以外は式文として処理する
            };
        }

        // let文を読んでStatementオブジェクトを生成する
        private LetStatement? ParseLetStatement()
        {
            // let文は "let <identifier> = <expression>;
            var statement = new LetStatement
            {
                Token = CurrentToken
            };

            if (!ExpectPeek(TokenType.IDENT))
                return null;

            // Identifier(識別子): let文の左辺
            statement.Name = new Identifier(CurrentToken, CurrentToken.Literal);

            // 等号 =
            if (!ExpectPeek(TokenType.ASSIGN))
                return null;

            // Expression（let文の右辺に当たる式）
            // TODO: let文の右辺式判定は未完成。一旦セミコロンまでを1Statementとして扱う
            while (CurrentToken.Type != TokenType.SEMICOLON)
            {
                // セミコロンが見つかるまで読み進める
                ReadToken();
            }

            return statement;
        }

        // return文を読み取る
        private ReturnStatement? ParseReturnStatement()
        {
            var statement = new ReturnStatement
            {
                // "return"部分をTokenとして登録する
                Token = CurrentToken
            };
            ReadToken();

            // TODO: 後で実装。一旦「;」まで読み取る実装にした
            while (CurrentToken.Type != TokenType.SEMICOLON)
            {
                // セミコロンが見つかるまで読み進める
                ReadToken();
            }

            return statement;
        }

        // 式を読み取る
        private ExpressionStatement? ParseExpressionStatement()
        {
            var statement = new ExpressionStatement
            {
                HeadToken = CurrentToken,
                Expression = ParseExpression(Precedence.Lowest)
            };

            // セミコロンを読み飛ばす（省略可能）
            if (NextToken.Type == TokenType.SEMICOLON)
            {
                ReadToken();
            }

            return statement;
        }

        private IExpression? ParseExpression(Precedence precedence)
        {
            PrefixParseFns.TryGetValue(CurrentToken.Type, out var prefix);
            if (prefix == null)
            {
                AddPrefixParseFnError(CurrentToken.Type);
                return null;
            }

            var leftExpression = prefix();
            return leftExpression;
        }

        private bool ExpectPeek(TokenType type)
        {
            // 次のトークンが期待するものであれば読み進める
            if (NextToken.Type == type)
            {
                ReadToken();
                return true;
            }

            // 文法エラーメッセージを登録する
            AddNextTokenError(type, NextToken.Type);
            return false;
        }

        private void RegisterPrefixParseFns()
        {
            PrefixParseFns = new Dictionary<TokenType, PrefixParseFn>();
            
            // 識別子
            PrefixParseFns.Add(TokenType.IDENT, ParseIdentifier);
            
            // 整数リテラル
            PrefixParseFns.Add(TokenType.INT, ParseIntegerLiteral);
            
            // 前置演算子
            PrefixParseFns.Add(TokenType.BANG, ParsePrefixExpression);  // 前置!演算子
            PrefixParseFns.Add(TokenType.MINUS, ParsePrefixExpression); // 前置-演算子
        }

        // 識別子式を生成する解析関数
        private IExpression ParseIdentifier()
        {
            return new Identifier(CurrentToken, CurrentToken.Literal);
        }

        // 整数リテラルを生成する
        private IExpression? ParseIntegerLiteral()
        {
            // 文字列リテラルを整数値に変換
            if (int.TryParse(CurrentToken.Literal, out int result))
            {
                return new IntegerLiteral()
                {
                    Token = CurrentToken,
                    Value = result,
                };
            }

            // 型変換失敗
            var message = $"{CurrentToken.Literal}をintegerに変換できません";
            Errors.Add(message);
            return null;
        }
        
        // 前置演算子を処理する
        private IExpression ParsePrefixExpression()
        {
            // 前置演算子部分を取得する
            var expression = new PrefixExpression()
            {
                Token = CurrentToken,
                Operator = CurrentToken.Literal,
            };

            ReadToken(); // 演算子の次のトークンまで読み進める
            expression.Right = ParseExpression(Precedence.Prefix);
            return expression;
        }

        private void AddNextTokenError(TokenType expected, TokenType actual)
        {
            Errors.Add($"{actual.ToString()}ではなく、{expected.ToString()}が渡されなければならない");
        }
        
        private void AddPrefixParseFnError(TokenType tokenType)
        {
            var message = $"{tokenType.ToString()}に関連付けられたPrefixParseFuncが存在しない";
            Errors.Add(message);
        }
    }
}