using CSharpLox;

namespace tests;

[Collection("Sequential")]
public class LexerTests
{
    [Fact]
    public void EmptyInput()
    {
        const string INPUT = "";
        var lexer = new Lexer(INPUT);
        var tokens = lexer.Tokens.ToList();

        Assert.Single(tokens);
        Assert.Equal(TokenType.EOF, tokens[0].Type);
    }

    [Fact]
    public void Whitespace()
    {
        const string INPUT = "    \t\n\t\n\r  ";
        var lexer = new Lexer(INPUT);
        var tokens = lexer.Tokens.ToList();

        Assert.Single(tokens);
        Assert.Equal(TokenType.EOF, tokens[0].Type);
    }

    [Fact]
    public void Tokenize()
    {
        const string INPUT = "((!)*</>-={}=={!={<=}>=;.,)/";
        var lexer = new Lexer(INPUT);
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
    public void TokenizeMultilineComment()
    {
        var input = """
                    !>*()== /* comment
                    another comment */
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

    [Fact]
    public void TokenizeNumber()
    {
        var input = """
                    123.456
                    123456
                    """;
        var lexer = new Lexer(input);
        var tokens = lexer.Tokens.ToList();
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.NUMBER, tokens[0].Type);
        Assert.Equal(123.456, tokens[0].Literal);
        Assert.Equal(TokenType.NUMBER, tokens[1].Type);
        Assert.Equal(123456D, tokens[1].Literal);
        Assert.Equal(TokenType.EOF, tokens[2].Type);
    }

    [Fact]
    public void TokenizeIdentifier()
    {
        const string INPUT = """
                             var x = 123;
                             var y = 456;
                             if (x > y or x < y) {
                                 print "x is greater than y";
                             } else {
                                 print "y is greater than x";
                             }
                             while (x != y and x > 0) {
                                 x = x - 1;
                                 y = y + 1;
                             }
                             class Test {
                                 var test = 123;
                                 fun main() {
                                     this.test = nil;
                                     super.test = true;
                                     super.test = false;
                                 }
                             }
                             """;

        var lexer = new Lexer(INPUT);
        var tokens = lexer.Tokens.ToList();
        Assert.Equal(89, tokens.Count);
        int pos = 0;
        Assert.Equal(TokenType.VAR, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("x", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.NUMBER, tokens[pos].Type);
        Assert.Equal(123D, tokens[pos++].Literal);
        Assert.Equal(TokenType.SEMICOLON, tokens[pos++].Type);
        Assert.Equal(TokenType.VAR, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("y", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.NUMBER, tokens[pos].Type);
        Assert.Equal(456D, tokens[pos++].Literal);
        Assert.Equal(TokenType.SEMICOLON, tokens[pos++].Type);
        Assert.Equal(TokenType.IF, tokens[pos++].Type);
        Assert.Equal(TokenType.LEFT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("x", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.GREATER, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("y", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.OR, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("x", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.LESS, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("y", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.RIGHT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.LEFT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.PRINT, tokens[pos++].Type);
        Assert.Equal(TokenType.STRING, tokens[pos].Type);
        Assert.Equal("x is greater than y", tokens[pos++].Literal);
        Assert.Equal(TokenType.SEMICOLON, tokens[pos++].Type);
        Assert.Equal(TokenType.RIGHT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.ELSE, tokens[pos++].Type);
        Assert.Equal(TokenType.LEFT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.PRINT, tokens[pos++].Type);
        Assert.Equal(TokenType.STRING, tokens[pos].Type);
        Assert.Equal("y is greater than x", tokens[pos++].Literal);
        Assert.Equal(TokenType.SEMICOLON, tokens[pos++].Type);
        Assert.Equal(TokenType.RIGHT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.WHILE, tokens[pos++].Type);
        Assert.Equal(TokenType.LEFT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("x", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.BANG_EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("y", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.AND, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("x", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.GREATER, tokens[pos++].Type);
        Assert.Equal(TokenType.NUMBER, tokens[pos].Type);
        Assert.Equal(0D, tokens[pos++].Literal);
        Assert.Equal(TokenType.RIGHT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.LEFT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("x", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("x", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.MINUS, tokens[pos++].Type);
        Assert.Equal(TokenType.NUMBER, tokens[pos].Type);
        Assert.Equal(1D, tokens[pos++].Literal);
        Assert.Equal(TokenType.SEMICOLON, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("y", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("y", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.PLUS, tokens[pos++].Type);
        Assert.Equal(TokenType.NUMBER, tokens[pos].Type);
        Assert.Equal(1D, tokens[pos++].Literal);
        Assert.Equal(TokenType.SEMICOLON, tokens[pos++].Type);
        Assert.Equal(TokenType.RIGHT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.CLASS, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("Test", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.LEFT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.VAR, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("test", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.NUMBER, tokens[pos].Type);
        Assert.Equal(123D, tokens[pos++].Literal);
        Assert.Equal(TokenType.SEMICOLON, tokens[pos++].Type);
        Assert.Equal(TokenType.FUN, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("main", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.LEFT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.RIGHT_PAREN, tokens[pos++].Type);
        Assert.Equal(TokenType.LEFT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.THIS, tokens[pos++].Type);
        Assert.Equal(TokenType.DOT, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("test", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.NIL, tokens[pos++].Type);
        Assert.Equal(TokenType.SEMICOLON, tokens[pos++].Type);
        Assert.Equal(TokenType.SUPER, tokens[pos++].Type);
        Assert.Equal(TokenType.DOT, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("test", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.TRUE, tokens[pos++].Type);
        Assert.Equal(TokenType.SEMICOLON, tokens[pos++].Type);
        Assert.Equal(TokenType.SUPER, tokens[pos++].Type);
        Assert.Equal(TokenType.DOT, tokens[pos++].Type);
        Assert.Equal(TokenType.IDENTIFIER, tokens[pos].Type);
        Assert.Equal("test", tokens[pos++].Lexeme);
        Assert.Equal(TokenType.EQUAL, tokens[pos++].Type);
        Assert.Equal(TokenType.FALSE, tokens[pos++].Type);
        Assert.Equal(TokenType.SEMICOLON, tokens[pos++].Type);
        Assert.Equal(TokenType.RIGHT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.RIGHT_BRACE, tokens[pos++].Type);
        Assert.Equal(TokenType.EOF, tokens[pos].Type);
    }
}