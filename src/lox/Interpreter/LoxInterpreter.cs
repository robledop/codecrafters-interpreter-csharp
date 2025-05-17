using CSharpLox.Interpreter.Functions;
using CSharpLox.Parser;
using static CSharpLox.TokenType;

namespace CSharpLox.Interpreter;

public class LoxInterpreter : IExprVisitor<object?>, IStmtVisitor<object?>
{
    readonly LoxEnvironment _globals = new();
    readonly Dictionary<IExpr, int> _locals = new();
    LoxEnvironment _environment;

    public LoxInterpreter()
    {
        _environment = _globals;
        _globals.Define("clock", new Clock());
    }

    public void Interpret(List<IStmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError e)
        {
            Lox.RuntimeError(e);
        }
    }

    public object? Evaluate(IExpr expr) => expr.Accept(this);

    void Execute(IStmt stmt) => stmt.Accept(this);

    public void ExecuteBlock(List<IStmt> statements, LoxEnvironment scopedEnvironment)
    {
        var previous = _environment;
        try
        {
            _environment = scopedEnvironment;
            foreach (var statement in statements)
                Execute(statement);
        }
        finally
        {
            _environment = previous;
        }
    }

    public void Resolve(IExpr expr, int depth) => _locals[expr] = depth;

    public object? VisitAssignExpression(Assign expr)
    {
        var value = Evaluate(expr.Value);
        if (_locals.TryGetValue(expr, out int distance))
        {
            _environment.AssignAt(distance, expr.Name, value);
        }
        else
        {
            _globals.Assign(expr.Name, value);
        }

        return value;
    }

    public object? VisitBinaryExpression(Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case MINUS:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! - (double)right!;
            case SLASH:
                CheckNumberOperands(expr.Op, left, right);
                if (right is 0)
                {
                    throw new RuntimeError(expr.Op, "Division by zero.");
                }

                return (double)left! / (double)right!;
            case STAR:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! * (double)right!;
            case PLUS:
                return left switch
                {
                    double l when right is double r => l + r,
                    string l when right is string r => l + r,
                    _ => throw new RuntimeError(expr.Op, "Operands must be two numbers or two strings.")
                };
            case GREATER:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! > (double)right!;
            case GREATER_EQUAL:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! >= (double)right!;
            case LESS:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! < (double)right!;
            case LESS_EQUAL:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left! <= (double)right!;
            case EQUAL_EQUAL:
                return IsEqual(left, right);
            case BANG_EQUAL:
                return !IsEqual(left, right);
            default:
                throw new RuntimeError(expr.Op, "Unexpected binary operator.");
        }
    }

    public object? VisitGroupingExpression(Grouping expr) => Evaluate(expr.Expression);

    public object? VisitLiteralExpression(Literal expr) => expr.Value;

    public object? VisitUnaryExpression(Unary expr)
    {
        var right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case MINUS:
                CheckNumberOperand(expr.Op, right);
                return -(double)right!;
            case BANG:
                return !IsTruthy(right);
            default:
                throw new RuntimeError(expr.Op, "Unexpected unary operator.");
        }
    }

    public object? VisitVariableExpression(Variable expr) => LookUpVariable(expr.Name, expr);

    public object? VisitLogicalExpression(Logical expr)
    {
        var left = Evaluate(expr.Left);

        if (expr.Op.Type == OR)
        {
            if (IsTruthy(left)) return left;
        }
        else
        {
            if (!IsTruthy(left)) return left;
        }

        return Evaluate(expr.Right);
    }

    public object? VisitCallExpression(Call expr)
    {
        var callee = Evaluate(expr.Callee);
        if (callee is not ICallable function)
        {
            throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
        }

        var arguments = expr
            .Arguments
            .Select(Evaluate)
            .OfType<object>()
            .ToList();

        if (arguments.Count != function.Arity())
        {
            throw new RuntimeError(expr.Paren,
                $"Expected {function.Arity()} arguments but got {arguments.Count}.");
        }

        return function.Call(this, arguments);
    }

    public object? VisitGetExpression(Get expr)
    {
        var obj = Evaluate(expr.Object);
        if (obj is not LoxInstance instance)
            throw new RuntimeError(expr.Name, "Only instances have properties.");

        return instance.Get(expr.Name);
    }

    public object? VisitSetExpression(Set expr)
    {
        var obj = Evaluate(expr.Object);
        if (obj is not LoxInstance instance)
            throw new RuntimeError(expr.Name, "Only instances have fields.");

        var value = Evaluate(expr.Value);
        instance.Set(expr.Name, value);
        return value;
    }

    public object? VisitThisExpression(This expr) => LookUpVariable(expr.Keyword, expr);

    public object? VisitSuperExpression(Super expr)
    {
        var distance = _locals[expr];
        var superclass = (LoxClass?)_environment.GetAt(distance, new Token(CLASS, "super", null, -1));
        var instance = (LoxInstance?)_environment.GetAt(distance - 1, new Token(THIS, "this", null, -1));
        var method = superclass?.FindMethod(expr.Method.Lexeme!);
        if (method is null)
            throw new RuntimeError(expr.Method, $"Undefined property '{expr.Method.Lexeme}'.");
        if (instance != null) return method.Bind(instance);

        throw new RuntimeError(expr.Method, "Superclass must be a class.");
    }

    public object? VisitBlockStatement(Block expr)
    {
        ExecuteBlock(expr.Statements, new LoxEnvironment(_environment));
        return null;
    }

    public object? VisitClassStatement(Class stmt)
    {
        object? superClass = null;

        if (stmt.SuperClass is not null)
        {
            superClass = Evaluate(stmt.SuperClass);
            if (superClass is not LoxClass)
                throw new RuntimeError(stmt.SuperClass.Name, "Superclass must be a class.");
        }

        _environment.Define(stmt.Name.Lexeme!, null);
        if (superClass is not null)
        {
            _environment = new LoxEnvironment(_environment);
            _environment.Define("super", superClass);
        }

        var methods = new Dictionary<string, LoxFunction>();
        foreach (var method in stmt.Methods)
        {
            var function = new LoxFunction(method, _environment, method.Name.Lexeme == "init");
            methods[method.Name.Lexeme!] = function;
        }

        var klass = new LoxClass(stmt.Name.Lexeme!, (LoxClass?)superClass, methods);
        if (superClass is not null)
        {
            _environment = _environment.Enclosing ??
                           throw new RuntimeError(stmt.Name, "Enclosing environment not found.");
        }

        _environment.Assign(stmt.Name, klass);
        return null;
    }

    public object? VisitExpressionStatement(StmtExpression expr)
    {
        Evaluate(expr.Expression);
        return null;
    }

    public object? VisitFunctionStatement(Function stmt)
    {
        var function = new LoxFunction(stmt, _environment);

        _environment.Define(stmt.Name.Lexeme!, function);
        return null;
    }

    public object? VisitIfStatement(If expr)
    {
        if (IsTruthy(Evaluate(expr.Condition)))
        {
            Execute(expr.ThenBranch);
        }
        else if (expr.ElseBranch is not null)
        {
            Execute(expr.ElseBranch);
        }

        return null;
    }

    public object? VisitPrintStatement(Print expr)
    {
        var value = Evaluate(expr.Expression);
        Console.WriteLine(Lox.Stringify(value));
        return null;
    }

    public object? VisitReturnStatement(ReturnStmt stmt)
    {
        object? value = null;
        if (stmt.Value is not null)
        {
            value = Evaluate(stmt.Value);
        }

        throw new ReturnException(value);
    }

    public object? VisitVarStatement(Var stmt)
    {
        object? value = null;
        if (stmt.Initializer is not null)
        {
            value = Evaluate(stmt.Initializer);
        }

        _environment.Define(stmt.Name.Lexeme!, value);
        return null;
    }

    public object? VisitWhileStatement(While stmt)
    {
        while (IsTruthy(Evaluate(stmt.Condition)))
        {
            try
            {
                Execute(stmt.Body);
            }
            catch (BreakException)
            {
                break;
            }
            catch (ContinueException)
            {
                // Continue to the next iteration of the loop.
            }
        }

        return null;
    }

    public object? VisitBreakStatement(Break stmt) =>
        throw new BreakException();

    public object? VisitContinueStatement(Continue stmt) =>
        throw new ContinueException();

    object? LookUpVariable(Token name, IExpr expr)
    {
        if (_locals.TryGetValue(expr, out var distance))
            return _environment.GetAt(distance, name);

        return _globals.Get(name);
    }


    void CheckNumberOperand(Token op, object? operand)
    {
        if (operand is not double)
        {
            throw new RuntimeError(op, "Operand must be a number.");
        }
    }

    void CheckNumberOperands(Token op, object? left, object? right)
    {
        if (left is not double || right is not double)
        {
            throw new RuntimeError(op, "Operands must be numbers.");
        }
    }

    bool IsEqual(object? a, object? b)
    {
        return a switch
        {
            null when b == null => true,
            null => false,
            _ => a.Equals(b)
        };
    }

    bool IsTruthy(object? value)
    {
        return value switch
        {
            null => false,
            bool b => b,
            _ => true
        };
    }
}