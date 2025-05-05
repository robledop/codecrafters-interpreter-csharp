using LoxInterpreter;
using LoxInterpreter.Parser;

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

        if (Lox.HadError)
        {
            Environment.Exit(65);
        }
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
        var expression = parser.Parse();
        if (expression == null)
        {
            Environment.Exit(65);
        }

        var printer = new AstPrinter();
        var result = printer.Print(expression);
        Console.WriteLine(result);
        if (Lox.HadError)
        {
            Environment.Exit(65);
        }
    }
        break;

    default:
        Console.Error.WriteLine($"Unknown command: {command}");
        Lox.PrintUsage();
        Environment.Exit(64);
        break;
}