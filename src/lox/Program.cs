using CSharpLox;
using Environment = System.Environment;

var command = args[0];

if (command == "repl")
{
    Lox.RunPrompt();
    return;
}

var filename = args.Length > 1 ? args[1] : null;

if (filename == null)
{
    Lox.PrintUsage();
    Environment.Exit(64);
}

var source = File.ReadAllText(filename);

switch (command)
{
    case "tokenize":
        Lox.Tokenize(source);
        break;

    case "parse":
        Lox.Parse(source);
        break;

    case "evaluate":
        Lox.Evaluate(source);
        break;

    case "run":
        Lox.Run(source);
        break;

    default:
        Console.Error.WriteLine($"Unknown command: {command}");
        Lox.PrintUsage();
        Environment.Exit(64);
        break;
}