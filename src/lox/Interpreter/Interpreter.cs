using LoxInterpreter.Parser;
using static LoxInterpreter.TokenType;

namespace LoxInterpreter.Interpreter;

public class Interpreter : IVisitor<object?>
{
    public object? Evaluate(IExpr expr)
    {
        return expr.Accept(this);
    }

    public object? Visit<TExpr>(TExpr expr) where TExpr : IExpr
    {
        return expr switch
        {
            Literal literal => literal.Value,
            Grouping grouping => Evaluate(grouping.Expr),
            Unary unary => VisitUnary(unary),
            Binary binary => VisitBinary(binary),
            _ => throw new ArgumentOutOfRangeException(nameof(expr), expr, null)
        };
    }

    object? VisitBinary(Binary binary)
    {
        var left = Evaluate(binary.Left);
        var right = Evaluate(binary.Right);

        switch (binary.Op.Type)
        {
            case MINUS:
                CheckNumberOperands(binary.Op, left, right);
                return (double)left! - (double)right!;
            case SLASH:
                CheckNumberOperands(binary.Op, left, right);
                return (double)left! / (double)right!;
            case STAR:
                CheckNumberOperands(binary.Op, left, right);
                return (double)left! * (double)right!;
            case PLUS:
                return left switch
                {
                    double l when right is double r => l + r,
                    string l when right is string r => l + r,
                    _ => throw new RuntimeError(binary.Op, "Operands must be two numbers or two strings.")
                };
            case GREATER:
                CheckNumberOperands(binary.Op, left, right);
                return (double)left! > (double)right!;
            case GREATER_EQUAL:
                CheckNumberOperands(binary.Op, left, right);
                return (double)left! >= (double)right!;
            case LESS:
                CheckNumberOperands(binary.Op, left, right);
                return (double)left! < (double)right!;
            case LESS_EQUAL:
                CheckNumberOperands(binary.Op, left, right);
                return (double)left! <= (double)right!;
            case EQUAL_EQUAL:
                return IsEqual(left, right);
            case BANG_EQUAL:
                return !IsEqual(left, right);
            default:
                return null;
        }
    }

    object? VisitUnary(Unary unary)
    {
        var right = Evaluate(unary.Right);

        switch (unary.Op.Type)
        {
            case MINUS:
                CheckNumberOperand(unary.Op, right);
                return -(double)right!;
            case BANG:
                return !IsTruthy(right);
            default:
                return null;
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