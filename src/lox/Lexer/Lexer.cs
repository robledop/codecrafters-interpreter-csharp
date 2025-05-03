namespace Lox;

public class Lexer
{
    public IEnumerable<Token> Tokens { get; set; }
    public int CurrentPosition { get; set; }
    private string Source { get; set; }
    private int NextPosition { get; set; }
    private char CurrentChar { get; set; }
    private int Line { get; set; } = 1;

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

    private void SkipWhitespace()
    {
        while (char.IsWhiteSpace(CurrentChar))
        {
            ReadChar();
        }
    }

    private Token NextToken()
    {
        SkipWhitespace();
        Token? token;

        switch (CurrentChar)
        {
            case '(':
                token = new Token { Type = TokenType.LEFT_PAREN, Literal = "(" };
                break;
            case ')':
                token = new Token { Type = TokenType.RIGHT_PAREN, Literal = ")" };
                break;
            case '{':
                token = new Token { Type = TokenType.LEFT_BRACE, Literal = "{" };
                break;
            case '}':
                token = new Token { Type = TokenType.RIGHT_BRACE, Literal = "}" };
                break;
            case ',':
                token = new Token { Type = TokenType.COMMA, Literal = "," };
                break;
            case '.':
                token = new Token { Type = TokenType.DOT, Literal = "." };
                break;
            case '-':
                token = new Token { Type = TokenType.MINUS, Literal = "-" };
                break;
            case '+':
                token = new Token { Type = TokenType.PLUS, Literal = "+" };
                break;
            case ';':
                token = new Token { Type = TokenType.SEMICOLON, Literal = ";" };
                break;
            case '*':
                token = new Token { Type = TokenType.STAR, Literal = "*" };
                break;
            case '\0':
                return new Token { Type = TokenType.EOF, Literal = "" };
            default:
                throw new Exception($"Unknown character: {CurrentChar}");
        }

        ReadChar();

        return token;
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

            yield return token;
        }
    }
}