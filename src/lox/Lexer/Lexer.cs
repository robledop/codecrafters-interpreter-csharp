namespace CSharpLox;

public class Lexer
{
    public IEnumerable<Token> Tokens { get; set; }
    int CurrentPosition { get; set; }
    string Source { get; set; }
    int NextPosition { get; set; }
    char CurrentChar { get; set; }
    int Line { get; set; } = 1;
    int Column { get; set; } = 1;

    static readonly Dictionary<string, TokenType> Keywords = new()
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
        { "while", TokenType.WHILE },
        { "break", TokenType.BREAK },
        { "continue", TokenType.CONTINUE },
    };

    public Lexer(string source)
    {
        Source = source;
        ReadChar();
        Tokens = ScanTokens();

        Reset();
        ReadChar();
    }

    void Reset()
    {
        CurrentPosition = 0;
        NextPosition = 0;
        CurrentChar = '\0';
    }

    void ReadChar()
    {
        CurrentChar = NextPosition >= Source.Length ? '\0' : Source[NextPosition];

        CurrentPosition = NextPosition;
        NextPosition++;
        Column++;
    }

    char Peek(int offset = 0)
    {
        return NextPosition + offset >= Source.Length ? '\0' : Source[NextPosition + offset];
    }

    bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (Source[NextPosition] != expected) return false;

        ReadChar();
        return true;
    }

    bool IsAtEnd()
    {
        return CurrentChar == '\0' || NextPosition >= Source.Length;
    }

    Token NextToken()
    {
        Token? token;

        switch (CurrentChar)
        {
            case '(':
                token = new Token(Type: TokenType.LEFT_PAREN, Lexeme: "(", Line: Line, Column: Column);
                break;
            case ')':
                token = new Token(Type: TokenType.RIGHT_PAREN, Lexeme: ")", Line: Line, Column: Column);
                break;
            case '{':
                token = new Token(Type: TokenType.LEFT_BRACE, Lexeme: "{", Line: Line, Column: Column);
                break;
            case '}':
                token = new Token(Type: TokenType.RIGHT_BRACE, Lexeme: "}", Line: Line, Column: Column);
                break;
            case ',':
                token = new Token(Type: TokenType.COMMA, Lexeme: ",", Line: Line, Column: Column);
                break;
            case '.':
                token = new Token(Type: TokenType.DOT, Lexeme: ".", Line: Line, Column: Column);
                break;
            case '-':
                token = new Token(Type: TokenType.MINUS, Lexeme: "-", Line: Line, Column: Column);
                break;
            case '+':
                token = new Token(Type: TokenType.PLUS, Lexeme: "+", Line: Line, Column: Column);
                break;
            case ';':
                token = new Token(Type: TokenType.SEMICOLON, Lexeme: ";", Line: Line, Column: Column);
                break;
            case '*':
                token = new Token(Type: TokenType.STAR, Lexeme: "*", Line: Line, Column: Column);
                break;
            case '!':
                token = Match('=')
                    ? new Token(Type: TokenType.BANG_EQUAL, Lexeme: "!=", Line: Line, Column: Column)
                    : new Token(Type: TokenType.BANG, Lexeme: "!", Line: Line, Column: Column);
                break;
            case '=':
                token = Match('=')
                    ? new Token(Type: TokenType.EQUAL_EQUAL, Lexeme: "==", Line: Line, Column: Column)
                    : new Token(Type: TokenType.EQUAL, Lexeme: "=", Line: Line, Column: Column);
                break;
            case '<':
                token = Match('=')
                    ? new Token(Type: TokenType.LESS_EQUAL, Lexeme: "<=", Line: Line, Column: Column)
                    : new Token(Type: TokenType.LESS, Lexeme: "<", Line: Line, Column: Column);
                break;
            case '>':
                token = Match('=')
                    ? new Token(Type: TokenType.GREATER_EQUAL, Lexeme: ">=", Line: Line, Column: Column)
                    : new Token(Type: TokenType.GREATER, Lexeme: ">", Line: Line, Column: Column);
                break;
            case '/':
                if (Match('/'))
                    token = ReadComment();
                else if (Match('*'))
                    token = ReadMultilineComment();
                else
                    token = new Token(Type: TokenType.SLASH, Lexeme: "/", Line: Line, Column: Column);

                break;

            case ' ':
            case '\r':
            case '\t':
                token = new Token(Type: TokenType.WHITESPACE, Lexeme: $"{CurrentChar}", Line: Line, Column: Column);
                break;

            case '\n':
                token = new Token(Type: TokenType.WHITESPACE, Lexeme: $"{CurrentChar}", Line: Line, Column: Column);
                Line++;
                Column = 1;
                break;
            case '"':
                token = ReadString();
                break;
            case '\0':
                return new Token(Type: TokenType.EOF, Line: Line, Column: Column);
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

                Lox.Error(Line, Column, $"Unexpected character: {CurrentChar}");
                token = new Token(Type: TokenType.INVALID, Line: Line, Column: Column);
                break;
        }

        ReadChar();

        return token;
    }

    Token ReadComment()
    {
        while (Peek() != '\n' && !IsAtEnd())
            ReadChar();

        return new Token(Type: TokenType.COMMENT, Line: Line, Column: Column);
    }

    Token ReadMultilineComment()
    {
        while (Peek() != '*' && Peek(1) != '/' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                Line++;
                Column = 1;
            }

            ReadChar();
        }

        if (IsAtEnd())
        {
            Lox.Error(Line, Column, "Unterminated block comment.");
            return new Token(Type: TokenType.INVALID, Line: Line, Column: Column);
        }

        ReadChar(); // Consume '*'
        ReadChar(); // Consume '/'
        return new Token(Type: TokenType.COMMENT, Line: Line, Column: Column);
    }

    Token ReadIdentifier()
    {
        var start = CurrentPosition;
        while (IsAlphanumeric(Peek())) ReadChar();

        var end = NextPosition;
        var lexeme = Source[start..end];

        return Keywords.TryGetValue(lexeme, out var type)
            ? new Token(Type: type, Lexeme: lexeme, Line: Line, Column: Column)
            : new Token(Type: TokenType.IDENTIFIER, Lexeme: lexeme, Line: Line, Column: Column);
    }

    bool IsAlphanumeric(char c) => char.IsLetterOrDigit(c) || c == '_';

    Token ReadString()
    {
        var start = NextPosition;
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                Line++;
                Column = 1;
            }

            ReadChar();
        }

        if (IsAtEnd())
        {
            Lox.Error(Line, Column, "Unterminated string.");
            return new Token(Type: TokenType.INVALID, Line: Line, Column: Column);
        }

        // Consume the closing "
        ReadChar();

        var end = CurrentPosition;
        var literal = Source[start..end];

        return new Token(Type: TokenType.STRING, Lexeme: $"\"{literal}\"", Literal: literal, Line: Line, Column: Column);
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
            return new Token(Type: TokenType.NUMBER, Lexeme: lexeme, Literal: number, Line: Line, Column: Column);
        }

        Lox.Error(Line, Column, $"Invalid number: {lexeme}");
        return new Token(Type: TokenType.INVALID, Lexeme: lexeme, Line: Line, Column: Column);
    }

    IEnumerable<Token> ScanTokens()
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