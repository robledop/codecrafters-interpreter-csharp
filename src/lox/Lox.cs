using System.Globalization;
using CSharpLox.Interpreter;
using CSharpLox.Parser;
using Environment = System.Environment;


namespace CSharpLox;

public static class Lox
{
    static readonly LoxInterpreter Interpreter = new();
    public static bool HadError { get; private set; }
    public static bool HadRuntimeError { get; private set; }

    public static void Error(int line, int column, string message)
    {
        Report(line, column, "", message);
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
            Report(token.Line, token.Column, " at end", message);
        else
            Report(token.Line, token.Column, $" at '{token.Lexeme}'", message);
    }

    public static void Report(int line, int column, string where, string message)
    {
        // Console.Error.WriteLine($"[line {line}, column {column}] Error{where}: {message}");
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        HadError = true;
    }

    public static void RuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
        HadRuntimeError = true;
    }

    public static IEnumerable<Token> GetTokens(string source)
    {
        HadError = false;
        HadRuntimeError = false;
        var lexer = new Lexer(source);
        var tokens = lexer.Tokens;

        return tokens;
    }

    public static void Tokenize(string source)
    {
        var tokens = GetTokens(source);

        foreach (var token in tokens)
        {
            if (token.Literal is double literal)
            {
                var number = literal % 1 == 0
                    ? literal.ToString("F1")
                    : literal.ToString("G");

                Console.WriteLine($"{token.Type} {token.Lexeme} {number}");
            }
            else
            {
                Console.WriteLine($"{token.Type} {token.Lexeme} {token.Literal ?? "null"}");
            }
        }

        if (HadError) Environment.Exit(65);
        if (HadRuntimeError) Environment.Exit(70);
    }

    public static IExpr? ParseExpression(List<Token> tokens)
    {
        var parser = new Parser.Parser(tokens);
        return parser.ParseExpression();
    }

    public static void Parse(string source)
    {
        var tokens = GetTokens(source);
        var parser = new Parser.Parser(tokens.ToList());
        var expression = parser.ParseExpression();
        if (expression == null)
        {
            Environment.Exit(65);
        }

        var printer = new AstPrinter();
        var result = printer.Print(expression);
        Console.WriteLine(result);
        if (HadError) Environment.Exit(65);
        if (HadRuntimeError) Environment.Exit(70);
    }

    public static void Run(string source, bool test = false)
    {
        try
        {
            var tokens = GetTokens(source);
            var parser = new Parser.Parser(tokens.ToList());
            var statements = parser.Parse().ToList();
            var resolver = new Resolver(Interpreter);
            resolver.Resolve(statements);

            if (!HadRuntimeError && !HadError)
            {
                Interpreter.Interpret(statements);
            }
        }
        catch (RuntimeError e)
        {
            RuntimeError(e);
        }

        if (!test)
        {
            if (HadError) Environment.Exit(65);
            if (HadRuntimeError) Environment.Exit(70);
        }
    }

    public static void Evaluate(string source)
    {
        HadError = false;
        HadRuntimeError = false;
        var lexer = new Lexer(source);

        var tokens = lexer.Tokens.ToList();
        var parser = new Parser.Parser(tokens);
        var expression = parser.ParseExpression();
        if (expression == null)
        {
            Environment.Exit(65);
        }

        try
        {
            var result = Interpreter.Evaluate(expression);
            Console.WriteLine(Stringify(result));
        }
        catch (RuntimeError e)
        {
            RuntimeError(e);
        }

        if (HadError) Environment.Exit(65);
        if (HadRuntimeError) Environment.Exit(70);
    }

    public static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == null)
                break;

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