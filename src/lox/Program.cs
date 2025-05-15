using System.Globalization;
using LoxInterpreter;
using LoxInterpreter.Interpreter;
using LoxInterpreter.Parser;
using Environment = System.Environment;

var command = args[0];
var filename = args.Length > 1 ? args[1] : null;

switch (command)
{
    case "tokenize":
    {
        if (filename == null)
        {
            Lox.PrintUsage();
            Environment.Exit(64);
        }

        var source = File.ReadAllText(filename);
        var tokens = Lox.Tokenize(source);

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

        if (Lox.HadError) Environment.Exit(65);
        if (Lox.HadRuntimeError) Environment.Exit(70);
    }

        break;

    case "repl":
        Lox.RunPrompt();
        break;

    case "parse":
    {
        if (filename == null)
        {
            Lox.PrintUsage();
            Environment.Exit(64);
        }

        var source = File.ReadAllText(filename);
        var tokens = Lox.Tokenize(source).ToList();
        var parser = new Parser(tokens);
        var expression = parser.ParseExpression();
        if (expression == null)
        {
            Environment.Exit(65);
        }

        var printer = new AstPrinter();
        var result = printer.Print(expression);
        Console.WriteLine(result);
        if (Lox.HadError) Environment.Exit(65);
        if (Lox.HadRuntimeError) Environment.Exit(70);
    }
        break;

    case "evaluate":
    {
        if (filename == null)
        {
            Lox.PrintUsage();
            Environment.Exit(64);
        }

        var source = File.ReadAllText(filename);
        var tokens = Lox.Tokenize(source).ToList();
        var parser = new Parser(tokens);
        var expression = parser.ParseExpression();
        if (expression == null)
        {
            Environment.Exit(65);
        }

        var interpreter = new Interpreter();
        try
        {
            var result = interpreter.Evaluate(expression);
            Console.WriteLine(Lox.Stringify(result));
        }
        catch (RuntimeError e)
        {
            Lox.RuntimeError(e);
        }

        if (Lox.HadError) Environment.Exit(65);
        if (Lox.HadRuntimeError) Environment.Exit(70);
    }
        break;

    case "run":
    {
        if (filename == null)
        {
            Lox.PrintUsage();
            Environment.Exit(64);
        }

        try
        {
            var source = File.ReadAllText(filename);
            var tokens = Lox.Tokenize(source).ToList();
            var parser = new Parser(tokens);
            var statements = parser.Parse();
            var interpreter = new Interpreter();
            var resolver = new Resolver(interpreter);
            resolver.Resolve(statements);
            if (!Lox.HadRuntimeError)
            {
                interpreter.Interpret(statements);
            }
        }
        catch (RuntimeError e)
        {
            Lox.RuntimeError(e);
        }

        if (Lox.HadError) Environment.Exit(65);
        if (Lox.HadRuntimeError) Environment.Exit(70);
    }

        break;

    default:
        Console.Error.WriteLine($"Unknown command: {command}");
        Lox.PrintUsage();
        Environment.Exit(64);
        break;
}