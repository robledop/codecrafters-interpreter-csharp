using System;
using System.IO;
using Lox;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: ./your_program.sh tokenize <filename>");
    Environment.Exit(1);
}

string command = args[0];
string filename = args[1];

if (command != "tokenize")
{
    Console.Error.WriteLine($"Unknown command: {command}");
    Environment.Exit(1);
}

string fileContents = File.ReadAllText(filename);

Console.Error.WriteLine("Logs from your program will appear here!");

if (!string.IsNullOrEmpty(fileContents))
{
    var lexer = new Lexer(fileContents);
    do
    {
        var token = lexer.NextToken();
        Console.WriteLine($"{token.Type} {token.Literal} null");
    } while (lexer.CurrentChar != '\0');

    Console.WriteLine("EOF  null");
}
else
{
    Console.WriteLine("EOF  null");
}