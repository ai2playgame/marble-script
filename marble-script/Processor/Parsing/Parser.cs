using Marble.Processor.AST;
using Marble.Processor.AST.Expression;
using Marble.Processor.AST.Statements;

namespace Marble.Processor.Parsing
{
    public class Parser
    {
        public Token CurrentToken { get; set; }
        public Token NextToken { get; set; }
        public Lexer Lexer { get; }
        public List<string> Errors { get; set; } = new List<string>();

        public Parser(Lexer lexer)
        {
            Lexer = lexer;
        
            // 2つ分のトークンを先に読み込んでおく
            CurrentToken = Lexer.NextToken();
            NextToken = Lexer.NextToken();
        }

        // 式をパースして、ASTを構築し、ルートに追加していく
        public Root ParseProgram()
        {
            var root = new Root();
            root.Statements = new List<IStatement>();
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
                _ => null
            };
        }

        // let文を読んでStatementオブジェクトを生成する
        private LetStatement? ParseLetStatement()
        {
            // let文は "let <identifier> = <expression>;
            var statement = new LetStatement();
            statement.Token = CurrentToken;

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
        private ReturnStatement ParseReturnStatement()
        {
            var statement = new ReturnStatement();
            // "return"部分をstatementに登録する
            statement.Token = CurrentToken;
            ReadToken();
            
            // TODO: 後で実装。一旦「;」まで読み取る実装にした
            while (CurrentToken.Type != TokenType.SEMICOLON)
            {
                // セミコロンが見つかるまで読み進める
                ReadToken();
            }

            return statement;
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

        private void AddNextTokenError(TokenType expected, TokenType actual)
        {
            Errors.Add($"{actual.ToString()}ではなく、{expected.ToString()}が渡されなければならない");
        }
    }
    
}