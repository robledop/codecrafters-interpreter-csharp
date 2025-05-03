namespace LoxInterpreter;

public static class Lox
{
    private static bool _hadError = false;

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        _hadError = true;
    }

    public static async Task RunFile(string path)
    {
        var contents = await File.ReadAllTextAsync(path);
        Run(contents);
        if (_hadError)
        {
            Environment.Exit(65);
        }
    }

    static void Run(string source)
    {
        var lexer = new Lexer(source);
        var tokens = lexer.Tokens;

        foreach (var token in tokens)
        {
            if (token.Literal is double)
            {
                Console.WriteLine($"{token.Type} {token.Lexeme} {token.Literal:D}");
            }
            else
            {
                Console.WriteLine($"{token.Type} {token.Lexeme} {token.Literal ?? "null"}");
            }
        }
    }

    public static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == null)
            {
                break;
            }

            Run(line);
            _hadError = false;
        }
    }

    public static void PrintUsage()
    {
        Console.WriteLine("Usage: ./lox tokenize <filename>");
        Console.WriteLine("       ./lox repl");
    }
}