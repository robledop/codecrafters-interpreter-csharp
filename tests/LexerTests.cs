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
        Assert.Equal("", tokens[0].Literal);
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
        Assert.Equal("", tokens[0].Literal);
    }

    [Fact]
    public void Tokenize()
    {
        const string input = "((!)*<>-={}=={!={<=}>=;.,)";
        var lexer = new Lexer(input);
        var tokens = lexer.Tokens.ToList();

        Assert.Equal(23, tokens.Count);
        Assert.Equal(TokenType.LEFT_PAREN, tokens[0].Type);
        Assert.Equal(TokenType.LEFT_PAREN, tokens[1].Type);
        Assert.Equal(TokenType.BANG, tokens[2].Type);
        Assert.Equal(TokenType.RIGHT_PAREN, tokens[3].Type);
        Assert.Equal(TokenType.STAR, tokens[4].Type);
        Assert.Equal(TokenType.LESS, tokens[5].Type);
        Assert.Equal(TokenType.GREATER, tokens[6].Type);
        Assert.Equal(TokenType.MINUS, tokens[7].Type);
        Assert.Equal(TokenType.EQUAL, tokens[8].Type);
        Assert.Equal(TokenType.LEFT_BRACE, tokens[9].Type);
        Assert.Equal(TokenType.RIGHT_BRACE, tokens[10].Type);
        Assert.Equal(TokenType.EQUAL_EQUAL, tokens[11].Type);
        Assert.Equal(TokenType.LEFT_BRACE, tokens[12].Type);
        Assert.Equal(TokenType.BANG_EQUAL, tokens[13].Type);
        Assert.Equal(TokenType.LEFT_BRACE, tokens[14].Type);
        Assert.Equal(TokenType.LESS_EQUAL, tokens[15].Type);
        Assert.Equal(TokenType.RIGHT_BRACE, tokens[16].Type);
        Assert.Equal(TokenType.GREATER_EQUAL, tokens[17].Type);
        Assert.Equal(TokenType.SEMICOLON, tokens[18].Type);
        Assert.Equal(TokenType.DOT, tokens[19].Type);
        Assert.Equal(TokenType.COMMA, tokens[20].Type);
        Assert.Equal(TokenType.RIGHT_PAREN, tokens[21].Type);
        Assert.Equal(TokenType.EOF, tokens[22].Type);
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
}