using CSharpLox;
using CSharpLox.Interpreter;
using CSharpLox.Parser;
using Xunit.Abstractions;
using static CSharpLox.TokenType;

namespace tests;

[Collection("Sequential")]
public class LoxInterpreterTests
{
    [Fact]
    public void TestBinaryExpressionAddition()
    {
        var left = new Literal(5.0);
        var right = new Literal(3.0);
        var op = new Token(PLUS, "+", null, 1);
        var binaryExpr = new Binary(left, op, right);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(binaryExpr);

        Assert.IsType<double>(result);
        Assert.Equal(8, (double)result);
    }

    [Fact]
    public void TestBinaryExpressionSubtraction()
    {
        var left = new Literal(5.0);
        var right = new Literal(3.0);
        var op = new Token(MINUS, "-", null, 1);
        var binaryExpr = new Binary(left, op, right);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(binaryExpr);

        Assert.IsType<double>(result);
        Assert.Equal(2, (double)result);
    }

    [Fact]
    public void TestBinaryExpressionMultiplication()
    {
        var left = new Literal(5.0);
        var right = new Literal(3.0);
        var op = new Token(STAR, "*", null, 1);
        var binaryExpr = new Binary(left, op, right);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(binaryExpr);

        Assert.IsType<double>(result);
        Assert.Equal(15, (double)result);
    }

    [Fact]
    public void TestBinaryExpressionDivision()
    {
        var left = new Literal(6.0);
        var right = new Literal(3.0);
        var op = new Token(SLASH, "/", null, 1);
        var binaryExpr = new Binary(left, op, right);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(binaryExpr);

        Assert.IsType<double>(result);
        Assert.Equal(2, (double)result);
    }

    [Fact]
    public void TestBinaryExpressionStringConcatenation()
    {
        var left = new Literal("Hello, ");
        var right = new Literal("world!");
        var op = new Token(PLUS, "+", null, 1);
        var binaryExpr = new Binary(left, op, right);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(binaryExpr);

        Assert.IsType<string>(result);
        Assert.Equal("Hello, world!", (string)result);
    }

    [Fact]
    public void TestBinaryExpressionGreaterThan()
    {
        var left = new Literal(5.0);
        var right = new Literal(3.0);
        var op = new Token(GREATER, ">", null, 1);
        var binaryExpr = new Binary(left, op, right);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(binaryExpr);

        Assert.IsType<bool>(result);
        Assert.True((bool)result);
    }

    [Fact]
    public void TestBinaryExpressionLessThan()
    {
        var left = new Literal(5.0);
        var right = new Literal(3.0);
        var op = new Token(LESS, "<", null, 1);
        var binaryExpr = new Binary(left, op, right);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(binaryExpr);

        Assert.IsType<bool>(result);
        Assert.False((bool)result);
    }

    [Fact]
    public void TestBinaryExpressionEqual()
    {
        var left = new Literal(5.0);
        var right = new Literal(5.0);
        var op = new Token(EQUAL_EQUAL, "==", null, 1);
        var binaryExpr = new Binary(left, op, right);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(binaryExpr);

        Assert.IsType<bool>(result);
        Assert.True((bool)result);
    }

    [Fact]
    public void TestBinaryExpressionNotEqual()
    {
        var left = new Literal(5.0);
        var right = new Literal(3.0);
        var op = new Token(BANG_EQUAL, "!=", null, 1);
        var binaryExpr = new Binary(left, op, right);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(binaryExpr);

        Assert.IsType<bool>(result);
        Assert.True((bool)result);
    }

    [Fact]
    public void TestUnaryExpressionNegation()
    {
        var right = new Literal(5.0);
        var op = new Token(MINUS, "-", null, 1);
        var unaryExpr = new Unary(op, right);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(unaryExpr);

        Assert.IsType<double>(result);
        Assert.Equal(-5, (double)result);
    }

    [Fact]
    public void TestUnaryExpressionLogicalNot()
    {
        var right = new Literal(true);
        var op = new Token(BANG, "!", null, 1);
        var unaryExpr = new Unary(op, right);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(unaryExpr);

        Assert.IsType<bool>(result);
        Assert.False((bool)result);
    }

    [Fact]
    public void TestGroupingExpression()
    {
        var inner = new Literal(5.0);
        var groupingExpr = new Grouping(inner);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(groupingExpr);

        Assert.IsType<double>(result);
        Assert.Equal(5, (double)result);
    }

    [Fact]
    public void TestLiteralExpression()
    {
        var literalExpr = new Literal(5.0);

        var interpreter = new LoxInterpreter();
        var result = interpreter.Evaluate(literalExpr);

        Assert.IsType<double>(result);
        Assert.Equal(5, (double)result);
    }
}