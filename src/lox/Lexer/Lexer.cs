namespace LoxInterpreter;

public class Lexer
{
    public IEnumerable<Token> Tokens { get; set; }
    private int CurrentPosition { get; set; }
    private string Source { get; set; }
    private int NextPosition { get; set; }
    private char CurrentChar { get; set; }
    private int Line { get; set; } = 1;

    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "and", TokenType.AND },
        { "class", TokenType.CLASS },
        { "else", TokenType.ELSE },
        { "false", TokenType.FALSE },
        { "for", TokenType.FOR },
        { "fun", TokenType.FUN },
        { "if", TokenType.IF },
        { "nil", TokenType.NIL },
        { "or", TokenType.OR },
        { "print", TokenType.PRINT },
        { "return", TokenType.RETURN },
        { "super", TokenType.SUPER },
        { "this", TokenType.THIS },
        { "true", TokenType.TRUE },
        { "var", TokenType.VAR },
        { "while", TokenType.WHILE }
    };

    public Lexer(string source)
    {
        Source = source;
        ReadChar();
        Tokens = ScanTokens();

        Reset();
        ReadChar();
    }

    private void Reset()
    {
        CurrentPosition = 0;
        NextPosition = 0;
        CurrentChar = '\0';
    }

    private void ReadChar()
    {
        CurrentChar = NextPosition >= Source.Length ? '\0' : Source[NextPosition];

        CurrentPosition = NextPosition;
        NextPosition++;
    }

    private char Peek(int offset = 0)
    {
        return NextPosition + offset >= Source.Length ? '\0' : Source[NextPosition + offset];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (Source[NextPosition] != expected) return false;

        ReadChar();
        return true;
    }

    private bool IsAtEnd()
    {
        return CurrentChar == '\0' || NextPosition >= Source.Length;
    }

    private Token NextToken()
    {
        Token? token;

        switch (CurrentChar)
        {
            case '(':
                token = new Token(Type: TokenType.LEFT_PAREN, Lexeme: "(", Line: Line);
                break;
            case ')':
                token = new Token(Type: TokenType.RIGHT_PAREN, Lexeme: ")", Line: Line);
                break;
            case '{':
                token = new Token(Type: TokenType.LEFT_BRACE, Lexeme: "{", Line: Line);
                break;
            case '}':
                token = new Token(Type: TokenType.RIGHT_BRACE, Lexeme: "}", Line: Line);
                break;
            case ',':
                token = new Token(Type: TokenType.COMMA, Lexeme: ",", Line: Line);
                break;
            case '.':
                token = new Token(Type: TokenType.DOT, Lexeme: ".", Line: Line);
                break;
            case '-':
                token = new Token(Type: TokenType.MINUS, Lexeme: "-", Line: Line);
                break;
            case '+':
                token = new Token(Type: TokenType.PLUS, Lexeme: "+", Line: Line);
                break;
            case ';':
                token = new Token(Type: TokenType.SEMICOLON, Lexeme: ";", Line: Line);
                break;
            case '*':
                token = new Token(Type: TokenType.STAR, Lexeme: "*", Line: Line);
                break;
            case '!':
                token = Match('=')
                    ? new Token(Type: TokenType.BANG_EQUAL, Lexeme: "!=", Line: Line)
                    : new Token(Type: TokenType.BANG, Lexeme: "!", Line: Line);
                break;
            case '=':
                token = Match('=')
                    ? new Token(Type: TokenType.EQUAL_EQUAL, Lexeme: "==", Line: Line)
                    : new Token(Type: TokenType.EQUAL, Lexeme: "=", Line: Line);
                break;
            case '<':
                token = Match('=')
                    ? new Token(Type: TokenType.LESS_EQUAL, Lexeme: "<=", Line: Line)
                    : new Token(Type: TokenType.LESS, Lexeme: "<", Line: Line);
                break;
            case '>':
                token = Match('=')
                    ? new Token(Type: TokenType.GREATER_EQUAL, Lexeme: ">=", Line: Line)
                    : new Token(Type: TokenType.GREATER, Lexeme: ">", Line: Line);
                break;
            case '/':
                if (Match('/'))
                    token = ReadComment();
                else if (Match('*'))
                    token = ReadMultilineComment();
                else
                    token = new Token(Type: TokenType.SLASH, Lexeme: "/", Line: Line);

                break;

            case ' ':
            case '\r':
            case '\t':
                token = new Token(Type: TokenType.WHITESPACE, Lexeme: $"{CurrentChar}", Line: Line);
                break;

            case '\n':
                token = new Token(Type: TokenType.WHITESPACE, Lexeme: $"{CurrentChar}", Line: Line);
                Line++;
                break;
            case '"':
                token = ReadString();
                break;
            case '\0':
                return new Token(Type: TokenType.EOF, Line: Line);
            default:
                if (char.IsDigit(CurrentChar))
                {
                    token = ReadNumber();
                    break;
                }

                if (IsAlphanumeric(CurrentChar))
                {
                    token = ReadIdentifier();
                    break;
                }

                Lox.Error(Line, $"Unexpected character: {CurrentChar}");
                token = new Token(Type: TokenType.INVALID, Line: Line);
                break;
        }

        ReadChar();

        return token;
    }

    Token ReadComment()
    {
        while (Peek() != '\n' && !IsAtEnd())
            ReadChar();

        return new Token(Type: TokenType.COMMENT, Line: Line);
    }

    Token ReadMultilineComment()
    {
        while (Peek() != '*' && Peek(1) != '/' && !IsAtEnd())
        {
            if (Peek() == '\n') Line++;

            ReadChar();
        }

        if (IsAtEnd())
        {
            Lox.Error(Line, "Unterminated block comment.");
            return new Token(Type: TokenType.INVALID, Line: Line);
        }

        ReadChar(); // Consume '*'
        ReadChar(); // Consume '/'
        return new Token(Type: TokenType.COMMENT, Line: Line);
    }

    Token ReadIdentifier()
    {
        var start = CurrentPosition;
        while (IsAlphanumeric(Peek())) ReadChar();

        var end = NextPosition;
        var lexeme = Source[start..end];

        return Keywords.TryGetValue(lexeme, out var type)
            ? new Token(Type: type, Lexeme: lexeme)
            : new Token(Type: TokenType.IDENTIFIER, Lexeme: lexeme, Line: Line);
    }

    bool IsAlphanumeric(char c) => char.IsLetterOrDigit(c) || c == '_';

    Token ReadString()
    {
        var start = NextPosition;
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') Line++;

            ReadChar();
        }

        if (IsAtEnd())
        {
            Lox.Error(Line, "Unterminated string.");
            return new Token(Type: TokenType.INVALID, Line: Line);
        }

        // Consume the closing "
        ReadChar();

        var end = CurrentPosition;
        var literal = Source[start..end];

        return new Token(Type: TokenType.STRING, Lexeme: $"\"{literal}\"", Literal: literal, Line: Line);
    }

    Token ReadNumber()
    {
        var start = CurrentPosition;
        while (char.IsDigit(Peek())) ReadChar();

        if (Peek() == '.' && char.IsDigit(Peek(1)))
        {
            ReadChar();
            while (char.IsDigit(Peek()))
                ReadChar();
        }

        var end = NextPosition;

        var lexeme = Source[start..end];
        if (double.TryParse(lexeme, out var number))
        {
            return new Token(Type: TokenType.NUMBER, Lexeme: lexeme, Literal: number, Line: Line);
        }

        Lox.Error(Line, $"Invalid number: {lexeme}");
        return new Token(Type: TokenType.INVALID, Lexeme: lexeme, Line: Line);
    }

    private IEnumerable<Token> ScanTokens()
    {
        while (true)
        {
            var token = NextToken();
            if (token.Type == TokenType.EOF)
            {
                yield return token;
                break;
            }

            if (token.Type is TokenType.INVALID or TokenType.COMMENT or TokenType.WHITESPACE)
            {
                continue;
            }

            yield return token;
        }
    }
}