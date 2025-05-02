namespace codecrafters_interpreter;

public static class Tokens
{
    public const string EOF = "EOF";
    public const string NUMBER = "NUMBER";
    public const string PLUS = "+";
    public const string MINUS = "-";
    public const string MULTIPLY = "*";
    public const string DIVIDE = "/";
    public const string LEFT_PAREN = "(";
    public const string RIGHT_PAREN = ")";
    public const string IDENTIFIER = "IDENTIFIER";
    public const string ASSIGN = "=";
    public const string SEMICOLON = ";";
    public const string PRINT = "PRINT";
    public const string IF = "IF";
    public const string ELSE = "ELSE";
}

public class Token
{
    public string Type { get; set; } = Tokens.EOF;
    public string? Literal { get; set; }
}