namespace Lox;

public class Lexer
{
    public string Input { get; set; }
    public int Position { get; set; }
    public int ReadPosition { get; set; }
    public char CurrentChar { get; set; }

    public Lexer(string input)
    {
        Input = input;
        ReadChar();
    }


    public void ReadChar()
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

    public Token NextToken()
    {
        SkipWhitespace();
        Token? token;

        switch (CurrentChar)
        {
            case '(':
                token = new Token { Type = Tokens.LEFT_PAREN, Literal = "(" };
                break;
            case ')':
                token = new Token { Type = Tokens.RIGHT_PAREN, Literal = ")" };
                break;
            case '\0':
                token = new Token { Type = Tokens.EOF, Literal = "" };
                break;
            default:
                throw new Exception($"Unknown character: {CurrentChar}");
        }

        ReadChar();

        return token;
    }
}