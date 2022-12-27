namespace Marble.Processor;

// REPL: Read-Eval-Print Loop
// インタプリタ言語において、対話的にプログラムを実行するもの
public class Repl
{
    private const string Prompt = ">> ";

    public void Start()
    {
        while (true)
        {
            Console.WriteLine(Prompt);

            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                return;

            var lexer = new Lexer(input);
            for (var token = lexer.NextToken();
                 token.Type != TokenType.EOF;
                 token = lexer.NextToken())
            {
                Console.WriteLine($"{{ Type: {token.Type}, Literal: {token.Literal} }}");
            }
        }
    }
}