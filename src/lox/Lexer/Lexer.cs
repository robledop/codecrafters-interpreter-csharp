namespace Lox;

public class Lexer
{
    string Input { get; set; }
    public int Position { get; set; }
    int ReadPosition { get; set; }
    char CurrentChar { get; set; }

    public Lexer(string input)
    {
        Input = input;
        ReadChar();
    }

    void ReadChar()
    {
        CurrentChar = ReadPosition >= Input.Length ? '\0' : Input[ReadPosition];

        Position = ReadPosition;
        ReadPosition++;
    }

    void SkipWhitespace()
    {
        while (char.IsWhiteSpace(CurrentChar))
        {
            ReadChar();
        }
    }

    Token NextToken()
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
            case '\0':
                token = new Token { Type = TokenType.EOF, Literal = "" };
                break;
            default:
                throw new Exception($"Unknown character: {CurrentChar}");
        }

        ReadChar();

        return token;
    }

    public IEnumerable<Token> ScanTokens()
    {
        while (true)
        {
            if (CurrentChar == '\0')
            {
                yield return NextToken();
                break;
            }

            yield return NextToken();
        }
    }
}