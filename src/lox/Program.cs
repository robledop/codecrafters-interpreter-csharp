using LoxInterpreter;

var command = args[0];
var filename = args.Length > 1 ? args[1] : null;

switch (command)
{
    case "tokenize":
        if (filename == null)
        {
            Lox.PrintUsage();
            Environment.Exit(64);
        }

        await Lox.RunFile(filename);
        break;
    case "repl":
        Lox.RunPrompt();
        break;
    default:
        Console.Error.WriteLine($"Unknown command: {command}");
        Lox.PrintUsage();
        Environment.Exit(64);
        break;
}