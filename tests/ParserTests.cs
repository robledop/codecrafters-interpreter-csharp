using CSharpLox;
using CSharpLox.Parser;

namespace tests;

[Collection("Sequential")]
public class ParserTests
{
    [Fact]
    public void OperatorPrecedenceAdditionMultiplication()
    {
        const string SOURCE = "1 + 2 * 3";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);

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

        Assert.False(Lox.HadError);
    }

    [Fact]
    public void OperatorPrecedenceAdditionDivision()
    {
        const string SOURCE = "1 + 2 / 3";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);

        Assert.NotNull(expression);
        Assert.IsType<Binary>(expression);
        var binaryExpr = (Binary)expression;
        Assert.Equal("+", binaryExpr.Op.Lexeme);
        Assert.IsType<Literal>(binaryExpr.Left);
        Assert.IsType<Binary>(binaryExpr.Right);
        var rightBinaryExpr = (Binary)binaryExpr.Right;
        Assert.Equal("/", rightBinaryExpr.Op.Lexeme);
        Assert.IsType<Literal>(rightBinaryExpr.Left);
        Assert.IsType<Literal>(rightBinaryExpr.Right);
        var rightLeftLiteralExpr = (Literal)rightBinaryExpr.Left;
        var rightRightLiteralExpr = (Literal)rightBinaryExpr.Right;
        Assert.Equal(2.0, rightLeftLiteralExpr.Value);
        Assert.Equal(3.0, rightRightLiteralExpr.Value);

        Assert.False(Lox.HadError);
    }

    [Fact]
    public void OperatorPrecedenceGroupingMultiplication()
    {
        const string SOURCE = "(1 + 2) * 3";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Binary>(expression);
        var binaryExpr = (Binary)expression;
        Assert.Equal("*", binaryExpr.Op.Lexeme);
        Assert.IsType<Grouping>(binaryExpr.Left);
        Assert.IsType<Literal>(binaryExpr.Right);
        var rightLiteralExpr = (Literal)binaryExpr.Right;
        Assert.Equal(3.0, rightLiteralExpr.Value);

        Assert.False(Lox.HadError);
    }

    [Fact]
    public void OperatorPrecedenceGroupingDivision()
    {
        const string SOURCE = "(1 + 2) / 3";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Binary>(expression);
        var binaryExpr = (Binary)expression;
        Assert.Equal("/", binaryExpr.Op.Lexeme);
        Assert.IsType<Grouping>(binaryExpr.Left);
        Assert.IsType<Literal>(binaryExpr.Right);
        var rightLiteralExpr = (Literal)binaryExpr.Right;
        Assert.Equal(3.0, rightLiteralExpr.Value);
        Assert.False(Lox.HadError);
    }

    [Fact]
    public void True()
    {
        const string SOURCE = "true";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);

        Assert.NotNull(expression);
        Assert.IsType<Literal>(expression);
        var literalExpr = (Literal)expression;
        Assert.NotNull(literalExpr.Value);
        Assert.Equal(true, literalExpr.Value);

        Assert.False(Lox.HadError);
    }

    [Fact]
    public void False()
    {
        const string SOURCE = "false";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Literal>(expression);
        var literalExpr = (Literal)expression;
        Assert.NotNull(literalExpr.Value);
        Assert.Equal(false, literalExpr.Value);
        Assert.False(Lox.HadError);
    }

    [Fact]
    public void UnterminatedString()
    {
        const string SOURCE = """
                              "bar" "unterminated
                              """;
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);

        Assert.NotNull(expression);
        Assert.IsType<Literal>(expression);
        var literalExpr = (Literal)expression;
        Assert.Equal("bar", literalExpr.Value);

        Assert.True(Lox.HadError);
    }

    [Fact]
    public void Number()
    {
        const string SOURCE = "81.0";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);

        Assert.NotNull(expression);
        Assert.IsType<Literal>(expression);
        var literalExpr = (Literal)expression;
        Assert.Equal(81.0, literalExpr.Value);

        Assert.False(Lox.HadError);
    }

    [Fact]
    public void Nil()
    {
        const string SOURCE = "nil";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Literal>(expression);
        var literalExpr = (Literal)expression;
        Assert.Null(literalExpr.Value);
        Assert.False(Lox.HadError);
    }

    [Fact]
    public void LessThan()
    {
        const string SOURCE = "1 < 2";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Binary>(expression);
        var binaryExpr = (Binary)expression;
        Assert.Equal("<", binaryExpr.Op.Lexeme);
        Assert.IsType<Literal>(binaryExpr.Left);
        Assert.IsType<Literal>(binaryExpr.Right);
        var leftLiteralExpr = (Literal)binaryExpr.Left;
        var rightLiteralExpr = (Literal)binaryExpr.Right;
        Assert.Equal(1.0, leftLiteralExpr.Value);
        Assert.Equal(2.0, rightLiteralExpr.Value);
        Assert.False(Lox.HadError);
    }

    [Fact]
    public void LessThanOrEqual()
    {
        const string SOURCE = "1 <= 2 ";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Binary>(expression);
        var binaryExpr = (Binary)expression;
        Assert.Equal("<=", binaryExpr.Op.Lexeme);
        Assert.IsType<Literal>(binaryExpr.Left);
        Assert.IsType<Literal>(binaryExpr.Right);
        var leftLiteralExpr = (Literal)binaryExpr.Left;
        var rightLiteralExpr = (Literal)binaryExpr.Right;
        Assert.Equal(1.0, leftLiteralExpr.Value);
        Assert.Equal(2.0, rightLiteralExpr.Value);

        Assert.False(Lox.HadError);
    }

    [Fact]
    public void GreaterThan()
    {
        const string SOURCE = "1 > 2 ";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Binary>(expression);
        var binaryExpr = (Binary)expression;
        Assert.Equal(">", binaryExpr.Op.Lexeme);
        Assert.IsType<Literal>(binaryExpr.Left);
        Assert.IsType<Literal>(binaryExpr.Right);
        var leftLiteralExpr = (Literal)binaryExpr.Left;
        var rightLiteralExpr = (Literal)binaryExpr.Right;
        Assert.Equal(1.0, leftLiteralExpr.Value);
        Assert.Equal(2.0, rightLiteralExpr.Value);

        Assert.False(Lox.HadError);
    }

    [Fact]
    public void GreaterThanOrEqual()
    {
        const string SOURCE = "1 >= 2 ";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Binary>(expression);
        var binaryExpr = (Binary)expression;
        Assert.Equal(">=", binaryExpr.Op.Lexeme);
        Assert.IsType<Literal>(binaryExpr.Left);
        Assert.IsType<Literal>(binaryExpr.Right);
        var leftLiteralExpr = (Literal)binaryExpr.Left;
        var rightLiteralExpr = (Literal)binaryExpr.Right;
        Assert.Equal(1.0, leftLiteralExpr.Value);
        Assert.Equal(2.0, rightLiteralExpr.Value);

        Assert.False(Lox.HadError);
    }

    [Fact]
    public void String()
    {
        const string SOURCE = "\"foo\"";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Literal>(expression);
        var literalExpr = (Literal)expression;
        Assert.Equal("foo", literalExpr.Value);

        Assert.False(Lox.HadError);
    }

    [Fact]
    public void Equal()
    {
        const string SOURCE = "1 == 2";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Binary>(expression);
        var binaryExpr = (Binary)expression;
        Assert.Equal("==", binaryExpr.Op.Lexeme);
        Assert.IsType<Literal>(binaryExpr.Left);
        Assert.IsType<Literal>(binaryExpr.Right);
        var leftLiteralExpr = (Literal)binaryExpr.Left;
        var rightLiteralExpr = (Literal)binaryExpr.Right;
        Assert.Equal(1.0, leftLiteralExpr.Value);
        Assert.Equal(2.0, rightLiteralExpr.Value);
        Assert.False(Lox.HadError);
    }

    [Fact]
    public void NotEqual()
    {
        const string SOURCE = "1 != 2";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Binary>(expression);
        var binaryExpr = (Binary)expression;
        Assert.Equal("!=", binaryExpr.Op.Lexeme);
        Assert.IsType<Literal>(binaryExpr.Left);
        Assert.IsType<Literal>(binaryExpr.Right);
        var leftLiteralExpr = (Literal)binaryExpr.Left;
        var rightLiteralExpr = (Literal)binaryExpr.Right;
        Assert.Equal(1.0, leftLiteralExpr.Value);
        Assert.Equal(2.0, rightLiteralExpr.Value);
        Assert.False(Lox.HadError);
    }

    [Fact]
    public void UnaryMinus()
    {
        const string SOURCE = "-1";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Unary>(expression);
        var unaryExpr = (Unary)expression;
        Assert.Equal("-", unaryExpr.Op.Lexeme);
        Assert.IsType<Literal>(unaryExpr.Right);
        var rightLiteralExpr = (Literal)unaryExpr.Right;
        Assert.Equal(1.0, rightLiteralExpr.Value);

        Assert.False(Lox.HadError);
    }

    [Fact]
    public void UnaryBang()
    {
        const string SOURCE = "!true";
        var tokens = Lox.GetTokens(SOURCE).ToList();
        var expression = Lox.ParseExpression(tokens);
        Assert.NotNull(expression);
        Assert.IsType<Unary>(expression);
        var unaryExpr = (Unary)expression;
        Assert.Equal("!", unaryExpr.Op.Lexeme);
        Assert.IsType<Literal>(unaryExpr.Right);
        var rightLiteralExpr = (Literal)unaryExpr.Right;
        Assert.Equal(true, rightLiteralExpr.Value);

        Assert.False(Lox.HadError);
    }
}