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

        public Dictionary<TokenType, Precedence> Precedences { get; set; } = new()
        {
            { TokenType.EQ, Precedence.EQUALS },
            { TokenType.NOT_EQ, Precedence.EQUALS },
            { TokenType.LT, Precedence.LESSGREATER },
            { TokenType.GT, Precedence.LESSGREATER },
            { TokenType.PLUS, Precedence.SUM },
            { TokenType.MINUS, Precedence.SUM },
            { TokenType.SLASH, Precedence.PRODUCT },
            { TokenType.ASTERISK, Precedence.PRODUCT },
            { TokenType.LPAREN, Precedence.CALL },
        };

        // 今見ているトークンの優先順位
        public Precedence CurrentPrecedence
        {
            get
            {
                if (Precedences.TryGetValue(CurrentToken.Type, out var p))
                {
                    return p;
                }

                return Precedence.LOWEST; // 優先順位未定義のトークンなら、優先順位は最低として扱う
            }
        }

        // 次のトークンの優先順位
        public Precedence NextPrecedence
        {
            get
            {
                if (Precedences.TryGetValue(NextToken.Type, out var p))
                {
                    return p;
                }

                return Precedence.LOWEST; // 優先順位未定義のトークンなら、優先順位は最低として扱う
            }
        }


        public Parser(Lexer lexer)
        {
            // フィールド初期化
            Lexer = lexer;
            Errors = new();

            // トークンの種別と、前置構文解析関数を紐づける
            RegisterPrefixParseFns();

            // トークンの種別と、中置構文解析関数を紐づける
            RegisterInfixParseFns();

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
            {
                return null;
            }

            // Identifier(識別子): let文の左辺
            statement.Name = new Identifier(CurrentToken, CurrentToken.Literal);

            // 等号 =
            if (!ExpectPeek(TokenType.ASSIGN))
            {
                return null;
            }

            // =を読み飛ばしてから、式を解析する
            ReadToken();
            statement.Value = ParseExpression(Precedence.LOWEST);
            // セミコロンは必須ではない
            if (NextToken.Type == TokenType.SEMICOLON)
            {
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

            // 式を解析する
            statement.ReturnValue = ParseExpression(Precedence.LOWEST);
            
            // セミコロンは必須ではない
            if (NextToken.Type == TokenType.SEMICOLON)
            {
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
                Expression = ParseExpression(Precedence.LOWEST)
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
            // 今見ているトークンに対応する前置解析関数を呼び出す
            PrefixParseFns.TryGetValue(CurrentToken.Type, out var prefix);
            if (prefix == null)
            {
                AddPrefixParseFnError(CurrentToken.Type);
                return null;
            }

            var leftExpression = prefix();

            // 中置演算子を、優先順位順に処理する
            while (NextToken.Type != TokenType.SEMICOLON &&
                   precedence < NextPrecedence)
            {
                InfixParseFns.TryGetValue(NextToken.Type, out var infix);
                if (infix == null)
                {
                    return leftExpression;
                }

                ReadToken();
                leftExpression = infix(leftExpression);
            }

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

            // 真偽値リテラル
            PrefixParseFns.Add(TokenType.TRUE, ParseBooleanLiteral);
            PrefixParseFns.Add(TokenType.FALSE, ParseBooleanLiteral);

            // 左括弧 "("
            PrefixParseFns.Add(TokenType.LPAREN, ParseGroupedExpression);

            // 左ブロック文括弧 "{"
            PrefixParseFns.Add(TokenType.IF, ParseIfExpression);

            // 関数リテラル "fn"
            PrefixParseFns.Add(TokenType.FUNCTION, ParseFunctionLiteral);
        }

        private void RegisterInfixParseFns()
        {
            InfixParseFns = new Dictionary<TokenType, InfixParseFn>();
            InfixParseFns.Add(TokenType.PLUS, ParseInfixExpression);
            InfixParseFns.Add(TokenType.MINUS, ParseInfixExpression);
            InfixParseFns.Add(TokenType.SLASH, ParseInfixExpression);
            InfixParseFns.Add(TokenType.ASTERISK, ParseInfixExpression);
            InfixParseFns.Add(TokenType.EQ, ParseInfixExpression);
            InfixParseFns.Add(TokenType.NOT_EQ, ParseInfixExpression);
            InfixParseFns.Add(TokenType.LT, ParseInfixExpression);
            InfixParseFns.Add(TokenType.GT, ParseInfixExpression);
            InfixParseFns.Add(TokenType.LPAREN, ParseCallExpression);
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

        // 真偽値リテラルを生成する
        private IExpression ParseBooleanLiteral()
        {
            return new BooleanLiteral()
            {
                Token = CurrentToken,
                Value = CurrentToken.Type == TokenType.TRUE,
            };
        }

        private IExpression? ParseIfExpression()
        {
            var expression = new IfExpression()
            {
                Token = CurrentToken, // IF
            };

            // ifの直後は括弧 (でなければならない
            if (!ExpectPeek(TokenType.LPAREN)) return null;

            // 括弧を読み飛ばす
            ReadToken();

            // ifの条件式を解析する
            expression.Condition = ParseExpression(Precedence.LOWEST);

            // ") {"と続く
            if (!ExpectPeek(TokenType.RPAREN)) return null;
            if (!ExpectPeek(TokenType.LBRACE)) return null;

            // {から始まるブロック文を解析する
            expression.Consequence = ParseBlockStatement();

            // else句がなければ、ここでreturnする
            if (NextToken.Type != TokenType.ELSE)
                return expression;

            // else句を解析する
            ReadToken(); // elseまで読み進める

            // elseの後は"{"
            if (!ExpectPeek(TokenType.LBRACE)) return null;
            // "{"から始まるelse句のブロック文を解析する
            expression.Alternative = ParseBlockStatement();

            return expression;
        }

        // 関数リテラルを解析する
        private IExpression? ParseFunctionLiteral()
        {
            var fn = new FunctionLiteral()
            {
                Token = CurrentToken,
            };

            // fnの後は左括弧
            if (!ExpectPeek(TokenType.LPAREN))
                return null;

            fn.Parameters = ParseParameters();

            // 関数パラメータの次はLBRACE
            if (!ExpectPeek(TokenType.LBRACE))
                return null;

            // 関数本体を解析する
            fn.Body = ParseBlockStatement();
            return fn;
        }

        private List<Identifier> ParseParameters()
        {
            var parameters = new List<Identifier>();

            // ()で、パラメータがない時は、空リストを返す
            if (NextToken.Type == TokenType.RPAREN)
            {
                ReadToken();
                return parameters;
            }

            ReadToken(); // (を読み飛ばす

            // 最初のパラメータをリストに追加する
            parameters.Add(new Identifier(CurrentToken, CurrentToken.Literal));

            // 2つ目以降のパラメータを、カンマが続く限りリストに追加し続ける
            while (NextToken.Type == TokenType.COMMA)
            {
                // リストに追加済の識別子と、カンマトークンを読み飛ばす
                ReadToken();
                ReadToken();

                // 識別子を追加する
                parameters.Add(new Identifier(CurrentToken, CurrentToken.Literal));
            }

            if (!ExpectPeek(TokenType.RPAREN))
                return null;

            return parameters;
        }

        // 括弧で囲むと演算の優先順位を調整できるように
        private IExpression? ParseGroupedExpression()
        {
            // "("を読み飛ばす
            ReadToken();

            // カッコ内の式を解析する
            var expression = ParseExpression(Precedence.LOWEST);

            // 閉じカッコ")"がないと、エラーになる
            if (!ExpectPeek(TokenType.RPAREN)) return null;

            return expression;
        }

        // ブロック文を解析する
        public BlockStatement ParseBlockStatement()
        {
            var block = new BlockStatement()
            {
                Token = CurrentToken, // "{"
                Statements = new List<IStatement>(),
            };

            // "{"を読み飛ばす
            ReadToken();

            while (CurrentToken.Type != TokenType.RBRACE &&
                   CurrentToken.Type != TokenType.EOF)
            {
                // ブロックの中身を解析する
                var statement = ParseStatement();
                if (statement != null)
                {
                    // 1文ずつParseして、Listに追加していく
                    block.Statements.Add(statement);
                }

                ReadToken();
            }

            return block;
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
            expression.Right = ParseExpression(Precedence.PREFIX);
            return expression;
        }

        // 中置演算子を処理する
        private IExpression ParseInfixExpression(IExpression lhs)
        {
            var expression = new InfixExpression()
            {
                Token = CurrentToken,
                Operator = CurrentToken.Literal,
                Lhs = lhs
            };

            var precedence = CurrentPrecedence;
            ReadToken();
            expression.Rhs = ParseExpression(precedence);

            return expression;
        }

        public IExpression ParseCallExpression(IExpression fn)
        {
            var expression = new CallExpression()
            {
                Token = CurrentToken,
                Function = fn,
                Arguments = ParseCallArguments(),
            };
            return expression;
        }

        public List<IExpression> ParseCallArguments()
        {
            var args = new List<IExpression>();
            
            // (を読み飛ばす
            ReadToken();
            
            // 引数なしの場合
            if (CurrentToken.Type == TokenType.RPAREN) { return args; }
            
            // 引数ありの場合は、まず1つ目の引数を解析する
            args.Add(ParseExpression(Precedence.LOWEST));
            
            // 2つ目以降の引数があればそれをリストに追加する
            while (NextToken.Type == TokenType.COMMA)
            {
                // カンマ直前のトークンとカンマトークンを読み飛ばす
                ReadToken();
                ReadToken();
                args.Add(ParseExpression(Precedence.LOWEST));
            }
            
            // 閉じ括弧がなければエラー
            if (!ExpectPeek(TokenType.RPAREN))
            {
                return null;
            }
            
            return args;
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