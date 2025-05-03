using LoxInterpreter;

namespace tests;

public class LexerTests
{
    [Fact]
    public void EmptyInput()
    {
        const string input = "";
        var lexer = new Lexer(input);
        var tokens = lexer.Tokens.ToList();

        Assert.Single(tokens);
        Assert.Equal(TokenType.EOF, tokens[0].Type);
    }

    [Fact]
    public void Whitespace()
    {
        const string input = "    \t\n\t\n\r  ";
        var lexer = new Lexer(input);
        var tokens = lexer.Tokens.ToList();
        ;

        Assert.Single(tokens);
        Assert.Equal(TokenType.EOF, tokens[0].Type);
    }

    [Fact]
    public void Tokenize()
    {
        const string input = "((!)*</>-={}=={!={<=}>=;.,)/";
        var lexer = new Lexer(input);
        var tokens = lexer.Tokens.ToList();

        int pos = 0;
        Assert.Equal(25, tokens.Count);
        Assert.Equal(TokenType.LEFT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.LEFT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.BANG, tokens[pos++].Type);
        Assert.Equal(TokenType.RIGHT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.STAR, tokens[pos++].Type);
        Assert.Equal(TokenType.LESS, tokens[pos++].Type);
        Assert.Equal(TokenType.SLASH, tokens[pos++].Type);
        Assert.Equal(TokenType.GREATER, tokens[pos++].Type);
        Assert.Equal(TokenType.MINUS, tokens[pos++].Type);
        Assert.Equal(TokenType.EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.LEFT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.RIGHT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.EQUAL_EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.LEFT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.BANG_EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.LEFT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.LESS_EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.RIGHT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.GREATER_EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.SEMICOLON, tokens[pos++].Type);
        Assert.Equal(TokenType.DOT, tokens[pos++].Type);
        Assert.Equal(TokenType.COMMA, tokens[pos++].Type);
        Assert.Equal(TokenType.RIGHT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.SLASH, tokens[pos++].Type);
        Assert.Equal(TokenType.EOF, tokens[pos].Type);
    }

    [Fact]
    public void TokenizeEqual()
    {
        var lexer = new Lexer("=");
        var tokens = lexer.Tokens.ToList();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.EQUAL, tokens[0].Type);
        Assert.Equal(TokenType.EOF, tokens[1].Type);
    }

    [Fact]
    public void TokenizeEqualEqual()
    {
        var lexer = new Lexer("==");
        var tokens = lexer.Tokens.ToList();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.EQUAL_EQUAL, tokens[0].Type);
        Assert.Equal(TokenType.EOF, tokens[1].Type);
    }

    [Fact]
    public void TokenizeComment()
    {
        var input = """
                    !>*()== // comment
                    // another comment
                    *)(/!
                    """;
        var lexer = new Lexer(input);
        var tokens = lexer.Tokens.ToList();

        int pos = 0;
        Assert.Equal(12, tokens.Count);
        Assert.Equal(TokenType.BANG, tokens[pos++].Type);
        Assert.Equal(TokenType.GREATER, tokens[pos++].Type);
        Assert.Equal(TokenType.STAR, tokens[pos++].Type);
        Assert.Equal(TokenType.LEFT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.RIGHT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.EQUAL_EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.STAR, tokens[pos++].Type);
        Assert.Equal(TokenType.RIGHT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.LEFT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.SLASH, tokens[pos++].Type);
        Assert.Equal(TokenType.BANG, tokens[pos++].Type);
        Assert.Equal(TokenType.EOF, tokens[pos].Type);
    }

    [Fact]
    public void TokenizeString()
    {
        var input = """
                    "Hello, World!"
                    """;
        var lexer = new Lexer(input);
        var tokens = lexer.Tokens.ToList();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.STRING, tokens[0].Type);
        Assert.Equal("Hello, World!", tokens[0].Literal);
        Assert.Equal(TokenType.EOF, tokens[1].Type);
    }
}