using LoxInterpreter;
using LoxInterpreter.Parser;
using Xunit.Abstractions;
using static LoxInterpreter.TokenType;

namespace tests;

[Collection("Sequential")]
public class InterpreterTests(ITestOutputHelper testOutput)
{
    [Fact]
    public void TestBinaryExpressionAddition()
    {
        var left = new Literal(5.0);
        var right = new Literal(3.0);
        var op = new Token(PLUS, "+", null, 1);
        var binaryExpr = new Binary(left, op, right);

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
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

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
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

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
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

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
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

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
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

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
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

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
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

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
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

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
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

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
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

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
        var result = interpreter.Evaluate(unaryExpr);

        Assert.IsType<bool>(result);
        Assert.False((bool)result);
    }

    [Fact]
    public void TestGroupingExpression()
    {
        var inner = new Literal(5.0);
        var groupingExpr = new Grouping(inner);

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
        var result = interpreter.Evaluate(groupingExpr);

        Assert.IsType<double>(result);
        Assert.Equal(5, (double)result);
    }

    [Fact]
    public void TestLiteralExpression()
    {
        var literalExpr = new Literal(5.0);

        var interpreter = new LoxInterpreter.Interpreter.Interpreter();
        var result = interpreter.Evaluate(literalExpr);

        Assert.IsType<double>(result);
        Assert.Equal(5, (double)result);
    }

    [Fact]
    public void Scope()
    {
        /* language=Java */
        const string CODE = """
                            var a = "global a";
                            var b = "global b";
                            var c = "global c";
                            {
                              var a = "outer a";
                              var b = "outer b";
                              {
                                var a = "inner a";
                                print a;
                                print b;
                                print c;
                              }
                              print a;
                              print b;
                              print c;
                            }

                            print a;
                            print b;
                            print c;
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);


        Lox.TestRun(CODE);
        var output = sw.ToString();

        const string EXPECTED_OUTPUT = """
                                       inner a
                                       outer b
                                       global c
                                       outer a
                                       outer b
                                       global c
                                       global a
                                       global b
                                       global c

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }

    [Fact]
    public void PreventImplicitDeclarations()
    {
        /* language=Java */
        const string CODE = "myVar = 1;";
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        const string EXPECTED_OUTPUT = """
                                       Undefined variable 'myVar'.
                                       [line 1]

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }

    [Fact]
    public void DefaultValue()
    {
        /* language=Java */
        const string CODE = """
                            var a;
                            print a;
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        const string EXPECTED_OUTPUT = """
                                       nil

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }

    [Fact]
    public void TestIfStatement()
    {
        /* language=Java */
        const string CODE = """
                            if (true) {
                                print "True branch";
                            } 
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        const string EXPECTED_OUTPUT = """
                                       True branch

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }

    [Fact]
    public void TestIfStatementWithElse()
    {
        /* language=Java */
        const string CODE = """
                            if (false) {
                                print "True branch";
                            } else {
                                print "False branch";
                            }
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        const string EXPECTED_OUTPUT = """
                                       False branch

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }

    [Fact]
    public void LogicalOr()
    {
        /* language=Java */
        const string CODE = """
                            if (true or false) {
                                print "True branch";
                            } else {
                                print "False branch";
                            }
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        const string EXPECTED_OUTPUT = """
                                       True branch

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }

    [Fact]
    public void LogicalAnd()
    {
        /* language=Java */
        const string CODE = """
                            if (true and false) {
                                print "True branch";
                            } else {
                                print "False branch";
                            }
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();


        const string EXPECTED_OUTPUT = """
                                       False branch

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }

    [Fact]
    public void WhileLoop()
    {
        /* language=Java */
        const string CODE = """
                            var i = 0;
                            while (i < 5) {
                                print i;
                                i = i + 1;
                            }
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        const string EXPECTED_OUTPUT = """
                                       0
                                       1
                                       2
                                       3
                                       4

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }

    [Fact]
    public void ForLoop()
    {
        /* language=Java */
        const string CODE = """
                            var a = 0;
                            var temp;

                            for (var b = 1; a < 10000; b = temp + b) {
                              print a;
                              temp = a;
                              a = b;
                            }
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        const string EXPECTED_OUTPUT = """
                                       0
                                       1
                                       1
                                       2
                                       3
                                       5
                                       8
                                       13
                                       21
                                       34
                                       55
                                       89
                                       144
                                       233
                                       377
                                       610
                                       987
                                       1597
                                       2584
                                       4181
                                       6765

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }

    [Fact]
    public void FunctionDeclaration()
    {
        /* language=Java */
        const string CODE = """
                            fun add(a, b) {
                            }

                            print add;
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        const string EXPECTED_OUTPUT = """
                                       <fn add>

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }

    [Fact]
    public void FunctionCall()
    {
        /* language=Java */
        const string CODE = """
                            fun sayHi(first, last) {
                              print "Hi, " + first + " " + last + "!";
                            }

                            sayHi("Dear", "Reader");
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        const string EXPECTED_OUTPUT = """
                                       Hi, Dear Reader!

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }

    [Fact]
    public void FunctionCallWithReturnAndRecursion()
    {
        /* language=Java */
        const string CODE = """
                            fun fib(n) {
                              if (n <= 1) return n;
                              return fib(n - 2) + fib(n - 1);
                            }

                            for (var i = 0; i < 10; i = i + 1) {
                              print fib(i);
                            }
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        const string EXPECTED_OUTPUT = """
                                       0
                                       1
                                       1
                                       2
                                       3
                                       5
                                       8
                                       13
                                       21
                                       34

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }

    [Fact]
    public void ClockFunction()
    {
        /* language=Java */
        const string CODE = """
                            var time = clock();
                            print time;
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        testOutput.WriteLine(output);
        Assert.Matches(@"^\d+$", output.Trim());
    }

    [Fact]
    public void ClockFunction2()
    {
        /* language=Java */
        const string CODE = """
                            print clock() + 80;
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        testOutput.WriteLine(output);
        Assert.Matches(@"^\d+$", output.Trim());
    }

    [Fact]
    public void FunctionWithClosure()
    {
        /* language=Java */
        const string CODE = """
                            fun makeCounter() {
                              var count = 0;
                              fun countUp() {
                                count = count + 1;
                                return count;
                              }
                              return countUp;
                            }

                            var counter = makeCounter();
                            print counter();
                            print counter();
                            print counter();
                            """;
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.TestRun(CODE);
        var output = sw.ToString();

        const string EXPECTED_OUTPUT = """
                                       1
                                       2
                                       3

                                       """;

        testOutput.WriteLine(output);
        Assert.Equal(EXPECTED_OUTPUT, output);
    }
}