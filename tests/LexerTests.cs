using Lox;

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
        var tokens = lexer.Tokens.ToList();;

        Assert.Single(tokens);
        Assert.Equal(TokenType.EOF, tokens[0].Type);
        Assert.Equal("", tokens[0].Literal);
    }

    [Fact]
    public void TokenizeParens()
    {
        const string input = "(()";
        var lexer = new Lexer(input);
        var tokens = lexer.Tokens.ToList();

        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.LEFT_PAREN, tokens[0].Type);
        Assert.Equal("(", tokens[0].Literal);
        Assert.Equal(TokenType.LEFT_PAREN, tokens[1].Type);
        Assert.Equal("(", tokens[1].Literal);
        Assert.Equal(TokenType.RIGHT_PAREN, tokens[2].Type);
        Assert.Equal(")", tokens[2].Literal);
        Assert.Equal(TokenType.EOF, tokens[3].Type);
        Assert.Equal("", tokens[3].Literal);
    }
}