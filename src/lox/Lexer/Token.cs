namespace CSharpLox;

public record Token(TokenType Type, string? Lexeme = null, object? Literal = null, int Line = 0, int Column = 0);