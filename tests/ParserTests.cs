using LoxInterpreter;
using LoxInterpreter.Parser;

namespace tests;

public class ParserTests
{
    [Fact]
    public void TestParse1()
    {
        var source = "1 + 2 * 3";
        var tokens = Lox.Tokenize(source).ToList();
        var parser = new Parser(tokens);
        var expression = parser.Parse();

        Assert.NotNull(expression);
        Assert.IsType<Binary>(expression);
        var binaryExpr = (Binary)expression;
        Assert.Equal("+", binaryExpr.Op.Lexeme);
        Assert.IsType<Literal>(binaryExpr.Left);
        Assert.IsType<Binary>(binaryExpr.Right);
        var rightBinaryExpr = (Binary)binaryExpr.Right;
        Assert.Equal("*", rightBinaryExpr.Op.Lexeme);
        Assert.IsType<Literal>(rightBinaryExpr.Left);
        Assert.IsType<Literal>(rightBinaryExpr.Right);
    }

    [Fact]
    public void TestParse2()
    {
        const string source = "true";
        var tokens = Lox.Tokenize(source).ToList();
        var parser = new Parser(tokens);
        var expression = parser.Parse();

        Assert.NotNull(expression);
        Assert.IsType<Literal>(expression);
        var literalExpr = (Literal)expression;
        Assert.NotNull(literalExpr.Value);
        Assert.Equal("true", literalExpr.Value.ToString());
    }

    [Fact]
    public void TestParse3()
    {
        const string source = """
                              "bar" "unterminated
                              """;
        var tokens = Lox.Tokenize(source).ToList();
        var parser = new Parser(tokens);
        var expression = parser.Parse();

        Assert.NotNull(expression);
        Assert.IsType<Literal>(expression);
        var literalExpr = (Literal)expression;
        Assert.Equal("bar", literalExpr.Value);

        Assert.True(Lox.HadError);
    }
}