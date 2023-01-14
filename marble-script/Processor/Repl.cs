using Marble.Processor.Parsing;

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
            {
                return;
            }

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();

            if (parser.Errors.Count > 0)
            {
                foreach (var error in parser.Errors)
                {
                    Console.WriteLine($"\t{error}");
                }
                continue;
            }
            
            Console.WriteLine(root.ToCode());
        }
    }
}