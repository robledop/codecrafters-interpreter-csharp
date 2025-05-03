namespace Lox;

public static class Tokens
{
    public const string EOF = nameof(EOF);
    public const string NUMBER = nameof(NUMBER);
    public const string PLUS = nameof(PLUS);
    public const string MINUS = nameof(MINUS);
    public const string LEFT_PAREN = nameof(LEFT_PAREN);
    public const string RIGHT_PAREN = nameof(RIGHT_PAREN);
}

public class Token
{
    public string Type { get; set; } = Tokens.EOF;
    public string? Literal { get; set; }
}