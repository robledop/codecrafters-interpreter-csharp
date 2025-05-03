using Lox;

bool hadError = false;
var command = args[0];
var filename = args.Length > 1 ? args[1] : null;

switch (command)
{
    case "tokenize":
        if (filename == null)
        {
            PrintUsage();
            Environment.Exit(64);
        }

        await RunFile(filename);
        break;
    case "repl":
        RunPrompt();
        break;
    default:
        Console.Error.WriteLine($"Unknown command: {command}");
        PrintUsage();
        Environment.Exit(64);
        break;
}

async Task RunFile(string path)
{
    var contents = await File.ReadAllTextAsync(path);
    Run(contents);
    if (hadError)
    {
        Environment.Exit(65);
    }
}

void Run(string source)
{
    var lexer = new Lexer(source);
    var tokens = lexer.ScanTokens();

    foreach (var token in tokens)
    {
        Console.WriteLine($"{token.Type} {token.Literal} null");
    }
}

void RunPrompt()
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
        hadError = false;
    }
}

void PrintUsage()
{
    Console.WriteLine("Usage: ./lox tokenize <filename>");
    Console.WriteLine("       ./lox repl");
}

void Error(int line, string message)
{
    Report(line, "", message);
}

void Report(int line, string where, string message)
{
    Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
    hadError = true;
}