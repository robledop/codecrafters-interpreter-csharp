namespace LoxInterpreter;

public record Token(TokenType Type, string? Lexeme = null, object? Literal = null, int Line = 0);