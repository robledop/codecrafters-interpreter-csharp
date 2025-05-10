using LoxInterpreter.Parser;
using static LoxInterpreter.TokenType;

namespace LoxInterpreter.Interpreter;

public class Interpreter : IExprVisitor<object?>, IStmtVisitor<object?>
{
    LoxEnvironment _environment = new();

    public object? Evaluate(IExpr expr)
    {
        return expr.Accept(this);
    }

    public void Interpret(List<IStmt?> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                if (statement != null) Execute(statement);
            }
        }
        catch (RuntimeError e)
        {
            Lox.RuntimeError(e);
        }
    }

    void Execute(IStmt stmt)
    {
        stmt.Accept(this);
    }

    public object? VisitAssignExpression(Assign expr)
    {
        var value = Evaluate(expr.Value);
        if (expr.Name.Lexeme is null)
        {
            throw new RuntimeError(expr.Name, "Variable name cannot be null.");
        }

        _environment.Assign(expr.Name, value);
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
                return null;
        }
    }

    public object? VisitGroupingExpression(Grouping expr) => Evaluate(expr.Expr);

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
                return null;
        }
    }

    public object? VisitVariableExpression(Variable expr)
        => _environment.Get(expr.Name);

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
        throw new NotImplementedException();
    }

    public object? VisitGetExpression(Get expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitSetExpression(Set expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitThisExpression(This expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitSuperExpression(Super expr)
    {
        throw new NotImplementedException();
    }


    public object? VisitBlockStatement(Block expr)
    {
        ExecuteBlock(expr.Statements, new LoxEnvironment(_environment));
        return null;
    }

    public object? VisitClassStatement(Class expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitExpressionStatement(StmtExpression expr)
    {
        Evaluate(expr.Expression);
        return null;
    }

    public object? VisitFunctionStatement(Function expr)
    {
        throw new NotImplementedException();
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

    public object? VisitReturnStatement(Return expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitVarStatement(Var expr)
    {
        object? value = null;
        if (expr.Initializer is not null)
        {
            value = Evaluate(expr.Initializer);
        }

        if (expr.Name.Lexeme is null)
        {
            throw new RuntimeError(expr.Name, "Variable name cannot be null.");
        }

        _environment.Define(expr.Name.Lexeme, value);
        return null;
    }

    public object? VisitWhileStatement(While expr)
    {
        while (IsTruthy(Evaluate(expr.Condition)))
        {
            Execute(expr.Body);
        }

        return null;
    }

    void ExecuteBlock(List<IStmt> statements, LoxEnvironment environment)
    {
        var previous = _environment;
        try
        {
            _environment = environment;
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = previous;
        }
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
        if (a == null && b == null) return true;
        if (a == null) return false;
        // if (b == null) return false;

        return a.Equals(b);
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