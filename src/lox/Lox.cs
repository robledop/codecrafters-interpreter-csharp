using LoxInterpreter.Parser;

namespace LoxInterpreter;

public static class Lox
{
    public static bool HadError { get; private set; } = false;

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        HadError = true;
    }

    public static async Task RunFile(string path)
    {
        var contents = await File.ReadAllTextAsync(path);
        Run(contents);
        if (HadError)
        {
            Environment.Exit(65);
        }
    }

    public static IEnumerable<Token> Tokenize(string source)
    {
        var lexer = new Lexer(source);
        return lexer.Tokens;
    }

    public static IExpr? Parse(List<Token> tokens)
    {
        var parser = new Parser.Parser(tokens);
        return parser.Parse();
    }

    static void Run(string source)
    {
        var tokens = Tokenize(source).ToList();
        Parse(tokens);
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
            HadError = false;
        }
    }

    public static void PrintUsage()
    {
        Console.WriteLine("Usage: ./lox tokenize <filename>");
        Console.WriteLine("       ./lox repl");
    }
}