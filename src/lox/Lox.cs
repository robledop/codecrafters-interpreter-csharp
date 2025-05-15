using System.Globalization;
using LoxInterpreter.Interpreter;
using LoxInterpreter.Parser;
using Environment = System.Environment;


namespace LoxInterpreter;

public static class Lox
{
    public static bool HadError { get; private set; }
    public static bool HadRuntimeError { get; private set; }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }

    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        HadError = true;
    }

    public static void RuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
        HadRuntimeError = true;
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
        HadError = false;
        HadRuntimeError = false;
        var lexer = new Lexer(source);
        return lexer.Tokens;
    }

    public static IExpr? ParseExpression(List<Token> tokens)
    {
        var parser = new Parser.Parser(tokens);
        return parser.ParseExpression();
    }

    public static void Run(string source)
    {
        try
        {
            var tokens = Tokenize(source).ToList();
            var parser = new Parser.Parser(tokens);
            var statements = parser.Parse().ToList();
            var interpreter = new Interpreter.Interpreter();
            var resolver = new Resolver(interpreter);
            resolver.Resolve(statements);
            if (statements is [StmtExpression expressionStmt])
            {
                var result = interpreter.Evaluate(expressionStmt.Expression);
                Console.WriteLine(Stringify(result));
            }

            interpreter.Interpret(statements);
        }
        catch (RuntimeError e)
        {
            RuntimeError(e);
        }

        if (HadError) Environment.Exit(65);
        if (HadRuntimeError) Environment.Exit(70);
    }

    public static void TestRun(string source)
    {
        try
        {
            var tokens = Tokenize(source).ToList();
            var parser = new Parser.Parser(tokens);
            var statements = parser.Parse().ToList();
            var interpreter = new Interpreter.Interpreter();
            var resolver = new Resolver(interpreter);
            resolver.Resolve(statements!);

            if (statements is [StmtExpression expressionStmt])
            {
                var result = interpreter.Evaluate(expressionStmt.Expression);
                Console.WriteLine(Stringify(result));
            }

            interpreter.Interpret(statements);
        }
        catch (RuntimeError e)
        {
            RuntimeError(e);
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
            HadError = false;
            HadRuntimeError = false;
        }
    }

    public static void PrintUsage()
    {
        Console.WriteLine("Usage: ./lox tokenize <filename>");
        Console.WriteLine("       ./lox repl");
    }

    public static string Stringify(object? obj)
    {
        switch (obj)
        {
            case double d:
                {
                    var text = d.ToString(CultureInfo.InvariantCulture);
                    if (text.EndsWith(".0"))
                    {
                        return text[0..^2];
                    }

                    break;
                }
            case bool b:
                return b ? "true" : "false";
            case null:
                return "nil";
        }

        return obj.ToString() ?? "nil";
    }
}