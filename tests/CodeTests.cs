using System.Text.RegularExpressions;
using CSharpLox;
using Xunit.Abstractions;

namespace tests;

[Collection("Sequential")]
public partial class CodeTests(ITestOutputHelper testOutput)
{
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

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void PreventImplicitDeclarations()
    {
        /* language=Java */
        const string CODE = "myVar = 1;";

        const string EXPECTED_OUTPUT = """
                                       Undefined variable 'myVar'.
                                       [line 1]

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void DefaultValue()
    {
        /* language=Java */
        const string CODE = """
                            var a;
                            print a;
                            """;

        const string EXPECTED_OUTPUT = """
                                       nil

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
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

        const string EXPECTED_OUTPUT = """
                                       True branch

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
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

        const string EXPECTED_OUTPUT = """
                                       False branch

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
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

        const string EXPECTED_OUTPUT = """
                                       True branch

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
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

        const string EXPECTED_OUTPUT = """
                                       False branch

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
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

        const string EXPECTED_OUTPUT = """
                                       0
                                       1
                                       2
                                       3
                                       4

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void Increment()
    {
        /* language=Java */
        const string CODE = """
                            var a = 1;
                            print a;
                            a = a + 1;
                            print a;
                            """;
        const string EXPECTED_OUTPUT = """
                                       1
                                       2

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
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

        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void ForLoop2()
    {
        /* language=Java */
        const string CODE = """
                            for (var quz = 0; quz < 3; quz = quz + 1) {
                               print quz;
                            }
                            """;
        const string EXPECTED_OUTPUT = """
                                       0
                                       1
                                       2

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void ForLoop3()
    {
        /* language=Java */
        const string CODE = """
                            for (var quz = 0; 
                                quz < 3; 
                                quz = quz + 1) {
                               print quz;
                            }
                            """;
        const string EXPECTED_OUTPUT = """
                                       0
                                       1
                                       2

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
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

        const string EXPECTED_OUTPUT = """
                                       <fn add>

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
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

        const string EXPECTED_OUTPUT = """
                                       Hi, Dear Reader!

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void FunctionCallNoParams()
    {
        /* language=Java */
        const string CODE = """
                            fun sayHi() {
                              print "Hi!";
                            }

                            sayHi();
                            """;

        const string EXPECTED_OUTPUT = """
                                       Hi!

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
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

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void FunctionWithReturn()
    {
        /* language=Java */
        const string CODE = """
                            fun fib(n) {
                              if (n <= 1) return n;
                              return fib(n - 2) + fib(n - 1);
                            }

                            var start = clock();
                            print fib(10) == 55;
                            print (clock() - start) < 5;
                            """;
        const string EXPECTED_OUTPUT = """
                                       true
                                       true

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void ClockFunction()
    {
        /* language=Java */
        const string CODE = """
                            var time = clock();
                            print time;
                            """;
        TestRun(CODE, NumberRegex());
    }

    [Fact]
    public void ClockFunction2()
    {
        /* language=Java */
        const string CODE = """
                            print clock() + 80;
                            """;

        TestRun(CODE, NumberRegex());
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

        const string EXPECTED_OUTPUT = """
                                       1
                                       2
                                       3

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void SyntaxErrorExpectExpression()
    {
        /* language=Java */
        const string CODE = """
                            clock(;
                            """;

        const string EXPECTED_OUTPUT = """
                                       [line 1] Error at ';': Expect expression.

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void SyntaxErrorExpectExpression2()
    {
        /* language=Java */
        const string CODE = """
                            print;
                            """;

        const string EXPECTED_OUTPUT = """
                                       [line 1] Error at ';': Expect expression.

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void SyntaxErrorExpectSemicolon()
    {
        /* language=Java */
        const string CODE = """
                            var a = 1
                            """;

        const string EXPECTED_OUTPUT = """
                                       [line 1] Error at end: Expect ';' after variable declaration.

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void SyntaxErrorExpectBlock()
    {
        /* language=Java */
        const string CODE = """
                            fun sayHi();
                            """;
        const string EXPECTED_OUTPUT = """
                                       [line 1] Error at ';': Expect '{' before function body.

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void SyntaxErrorExpectFunctionName()
    {
        /* language=Java */
        const string CODE = """
                            fun () {
                              print "Hello";
                            }
                            """;

        const string EXPECTED_OUTPUT = """
                                       [line 1] Error at '(': Expect function name.
                                       [line 3] Error at '}': Expect expression.

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void SyntaxErrorFunctionNotCallable()
    {
        /* language=Java */
        const string CODE = """
                            85();

                            """;
        const string EXPECTED_OUTPUT = """
                                       Can only call functions and classes.
                                       [line 1]

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void SyntaxErrorFunctionNotCallable2()
    {
        /* language=Java */
        const string CODE = """
                            false();

                            """;
        const string EXPECTED_OUTPUT = """
                                       Can only call functions and classes.
                                       [line 1]

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void SyntaxErrorFunctionCallTooManyArguments()
    {
        /* language=Java */
        const string CODE = """
                            fun add(a, b) {
                              return a + b;
                            }

                            print add(1, 2, 3);
                            """;
        const string EXPECTED_OUTPUT = """
                                       Expected 2 arguments but got 3.
                                       [line 5]

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void SyntaxErrorFunctionCallTooFewArguments()
    {
        /* language=Java */
        const string CODE = """
                            fun add(a, b) {
                              return a + b;
                            }

                            print add(1);
                            """;
        const string EXPECTED_OUTPUT = """
                                       Expected 2 arguments but got 1.
                                       [line 5]

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void HigherOrderFunction()
    {
        /* language=Java */
        const string CODE = """
                            fun returnArg(arg) {
                              return arg;
                            }

                            fun returnFunCallWithArg(func, arg) {
                              return returnArg(func)(arg);
                            }

                            fun printArg(arg) {
                              print arg;
                            }

                            returnFunCallWithArg(printArg, "foo");
                            """;

        const string EXPECTED_OUTPUT = """
                                       foo

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void FunctionScope()
    {
        /* language=Java */
        const string CODE = """
                            var a = 20;
                            {
                              print a;
                              var a = 42;
                              print a;
                            }
                            print a;
                            """;
        const string EXPECTED_OUTPUT = """
                                       20
                                       42
                                       20

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void FunctionScope2()
    {
        /* language=Java */
        const string CODE = """
                            var x = 1;
                            var y = 2;

                            fun printBoth() {
                              if (x < y) {
                                print "x is less than y:";
                                print x;
                                print y;
                              } else {
                                print "x is not less than y:";
                                print x;
                                print y;
                              }
                            }

                            {
                              var x = 10;
                              {
                                var y = 20;

                                var i = 0;
                                while (i < 3) {
                                  x = x + 1;
                                  y = y - 1;
                                  print "Local x: ";
                                  print x;
                                  print "Local y: ";
                                  print y;
                                  i = i + 1;
                                }

                                if (x > y) {
                                  print "Local x > y";
                                }

                                printBoth();
                              }
                            }
                            """;
        const string EXPECTED_OUTPUT = """
                                       Local x: 
                                       11
                                       Local y: 
                                       19
                                       Local x: 
                                       12
                                       Local y: 
                                       18
                                       Local x: 
                                       13
                                       Local y: 
                                       17
                                       x is less than y:
                                       1
                                       2

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void FunctionScope4()
    {
        /* language=Java */
        const string CODE = """
                            // This program tests that scopes can be nested
                            {
                                var baz = 61;
                                {
                                    var hello = 61;
                                    print hello;
                                }
                                print baz;
                            }
                            """;
        const string EXPECTED_OUTPUT = """
                                       61
                                       61

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact(Skip = "TODO")]
    public void AnonymousFunction()
    {
        /* language=Java */
        const string CODE = """
                            fun thrice(fn) {
                              for (var i = 1; i <= 3; i = i + 1) {
                                fn(i);
                              }
                            }

                            thrice(fun (a) {
                              print a;
                            });
                            """;

        const string EXPECTED_OUTPUT = """
                                       1
                                       2
                                       3

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void ClosureBinding()
    {
        /* language=Java */
        const string CODE = """
                            var a = "global";
                            {
                              fun showA() {
                                print a;
                              }

                              showA();
                              var a = "block";
                              showA();
                            }
                            """;

        const string EXPECTED_OUTPUT = """
                                       global
                                       global

                                       """;

        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void Break()
    {
        /* language=Java */
        const string CODE = """
                            var i = 0;
                            while (i < 10) {
                              if (i == 5) break;
                              print i;
                              i = i + 1;
                            }
                            """;
        const string EXPECTED_OUTPUT = """
                                       0
                                       1
                                       2
                                       3
                                       4

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void Continue()
    {
        /* language=Java */
        const string CODE = """
                            var i = 0;
                            while (i < 10) {
                              i = i + 1;
                              if (i == 5) continue;
                              print i;
                            }
                            """;
        const string EXPECTED_OUTPUT = """
                                       1
                                       2
                                       3
                                       4
                                       6
                                       7
                                       8
                                       9
                                       10

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void Binding()
    {
        /* language=Java */
        const string CODE = """
                            // This program uses a for loop to print the
                            // numbers from 0 to 2
                            // The loop initializer is ignored in this loop
                            var hello = 0;
                            for (; hello < 2; hello = hello + 1) print hello;

                            // This program uses a for loop to print the
                            // numbers from 0 to 2
                            // The loop increment clause is ignored in this
                            // loop
                            for (var world = 0; world < 2;) {
                              print world;
                             world = world + 1;
                            }
                            """;
        const string EXPECTED_OUTPUT = """
                                       0
                                       1
                                       0
                                       1

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void SelfInitialization1()
    {
        /* language=Java */
        const string CODE = """
                            // Helper function that simply returns its argument
                            fun returnArg(arg) {
                              return arg;
                            }

                            // Declare global variable 'b'
                            var b = "global";

                            {
                              // Local variable declaration
                              var a = "first";

                              // Attempting to initialize local variable 'b'
                              // using local variable 'b'
                              // through a function call
                              var b = returnArg(b); // expect compile error
                              print b;
                            }

                            var b = b + " updated";
                            print b;
                            """;
        const string EXPECTED_OUTPUT = """
                                       [line 16] Error at 'b': Can't read local variable in its own initializer.

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void InvalidReturn()
    {
        /* language=Java */
        const string CODE = """
                            fun foo() {
                              return "at function scope is ok";
                            }

                            return;
                            """;
        const string EXPECTED_OUTPUT = """
                                       [line 5] Error at 'return': Can't return from top-level code.

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void ClassDeclaration()
    {
        /* language=Java */
        const string CODE = """
                            class DevonshireCream { }
                            print DevonshireCream;
                            """;
        const string EXPECTED_OUTPUT = """
                                       DevonshireCream

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void ClassInstantiation()
    {
        /* language=Java */
        const string CODE = """
                            class Bagel {}
                            var bagel = Bagel();
                            print bagel; 
                            """;
        const string EXPECTED_OUTPUT = """
                                       Bagel instance

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void ClassMethod()
    {
        /* language=Java */
        const string CODE = """
                            class Bacon {
                              eat() {
                                print "Crunch crunch crunch!";
                              }
                            }

                            Bacon().eat();
                            """;
        const string EXPECTED_OUTPUT = """
                                       Crunch crunch crunch!

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void ClassGetterAndSetter()
    {
        /* language=Java */
        const string CODE = """
                            class Box {
                            }

                            var box = Box();
                            box.size = 10;
                            print box.size;

                            """;
        const string EXPECTED_OUTPUT = """
                                       10

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void This1()
    {
        /* language=Java */
        const string CODE = """
                            class Cake {
                              taste() {
                                var adjective = "delicious";
                                print "The " + this.flavor + " cake is " + adjective + "!";
                              }
                            }

                            var cake = Cake();
                            cake.flavor = "German chocolate";
                            cake.taste();
                            """;
        const string EXPECTED_OUTPUT = """
                                       The German chocolate cake is delicious!

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void This2()
    {
        /* language=Java */
        const string CODE = """
                            class Cake {
                              taste() {
                                var adjective = "delicious";
                                print "The " + this.flavor + " cake is " + adjective + "!";
                              }
                            }

                            var cake = Cake();
                            cake.flavor = "German chocolate";
                            var method = cake.taste;
                            method();
                            """;
        const string EXPECTED_OUTPUT = """
                                       The German chocolate cake is delicious!

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void ThisOutsideClass1()
    {
        /* language=Java */
        const string CODE = """
                            print this;
                            """;
        const string EXPECTED_OUTPUT = """
                                       [line 1] Error at 'this': Can't use 'this' outside of a class.

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void ThisOutsideClass2()
    {
        /* language=Java */
        const string CODE = """
                            fun notAMethod() {
                              print this;
                            }
                            
                            notAMethod();
                            """;
        const string EXPECTED_OUTPUT = """
                                       [line 2] Error at 'this': Can't use 'this' outside of a class.

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void Init()
    {
        /* language=Java */
        const string CODE = """
                            class Foo {
                              init() {
                                this.bar = 42;
                              }
                            }
                            var foo = Foo();
                            print foo.bar;
                            """;
        const string EXPECTED_OUTPUT = """
                                       42

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void InitMustNotReturn()
    {
        /* language=Java */
        const string CODE = """
                            class Foo {
                              init() {
                                return "something else";
                              }
                            }
                            var foo = Foo();
                            """;
        const string EXPECTED_OUTPUT = """
                                       [line 3] Error at 'return': Cannot return a value from an initializer.

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void InitEarlyReturnShouldReturnThis()
    {
        /* language=Java */
        const string CODE = """
                            class Foo {
                              init(a) {
                                if (a == 1) return;
                                print "Not 1";
                              }
                            }
                            var foo = Foo(1);
                            print foo.init(1);
                            """;
        const string EXPECTED_OUTPUT = """
                                       Foo instance

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void InitReturnsThis()
    {
        /* language=Java */
        const string CODE = """
                            class Foo {
                              init() {
                                this.bar = 42;
                              }
                            }
                            var foo = Foo();
                            print foo.init();
                            """;
        const string EXPECTED_OUTPUT = """
                                       Foo instance

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    [Fact]
    public void ClassHierarchy()
    {
        /* language=Java */
        const string CODE = """
                            class Doughnut {}

                            class BostonCream < Doughnut {}

                            print Doughnut();
                            print BostonCream();
                            """;
        const string EXPECTED_OUTPUT = """
                                       Doughnut instance
                                       BostonCream instance

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void ClassMustInheritFromClass()
    {
        /* language=Java */
        const string CODE = """
                            var NotAClass = "I am totally not a class";
                            class Subclass < NotAClass {}
                            """;
        const string EXPECTED_OUTPUT = """
                                       Superclass must be a class.
                                       [line 2]

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void ClassInheritMethods()
    {
        /* language=Java */
        const string CODE = """
                            class Doughnut {
                              cook() {
                                print "Fry until golden brown.";
                              }
                            }
                            
                            class BostonCream < Doughnut {}
                            
                            BostonCream().cook();
                            """;
        const string EXPECTED_OUTPUT = """
                                       Fry until golden brown.

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void ClassInheritMethodOverride()
    {
        /* language=Java */
        const string CODE = """
                            class A {
                              method() {
                                print "A method";
                              }
                            }
                            
                            // B inherits method `method` from A
                            // and overrides it with a new implementation
                            class B < A {
                              method() {
                                print "B method";
                              }
                            }
                            
                            var b = B();
                            b.method();
                            """;
        const string EXPECTED_OUTPUT = """
                                       B method

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void Super()
    {
        /* language=Java */
        const string CODE = """
                            class Doughnut {
                              cook() {
                                print "Fry until golden brown.";
                              }
                            }
                            
                            // Super can be used to call the overridden method
                            // of the parent class
                            class BostonCream < Doughnut {
                              cook() {
                                super.cook();
                              }
                            }
                            
                            BostonCream().cook();
                            """;
        const string EXPECTED_OUTPUT = """
                                       Fry until golden brown.

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }


    [Fact]
    public void VariableRedeclaration()
    {
        /* language=Java */
        const string CODE = """
                            {
                                var a = 1;
                                var a = 2;
                            }
                            """;
        const string EXPECTED_OUTPUT = """
                                       [line 3] Error at 'a': Already a variable with this name in this scope.

                                       """;
        TestRun(CODE, EXPECTED_OUTPUT);
    }

    void TestRun(string code, string expected)
    {
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.Run(code, test: true);
        var output = sw.ToString();

        testOutput.WriteLine(output);
        Assert.Equal(expected, output);
    }

    void TestRun(string code, Regex expected)
    {
        var sw = new StringWriter();
        Console.SetOut(sw);
        Console.SetError(sw);

        Lox.Run(code, test: true);
        var output = sw.ToString();

        testOutput.WriteLine(output);
        Assert.Matches(expected, output);
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();
}