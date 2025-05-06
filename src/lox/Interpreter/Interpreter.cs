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

        object? VisitUnary(Unary unary)
        {
            var right = Evaluate(unary.Right);

            return unary.Op.Type switch
            {
                MINUS => -(double)right!,
                BANG => !IsTruthy(right),
                _ => null
            };
        }

        object? VisitBinary(Binary binary)
        {
            var left = Evaluate(binary.Left);
            var right = Evaluate(binary.Right);

            return binary.Op.Type switch
            {
                MINUS => (double)left! - (double)right!,
                SLASH => (double)left! / (double)right!,
                STAR => (double)left! * (double)right!,
                PLUS => left switch
                {
                    double l when right is double r => l + r,
                    string l when right is string r => l + r,
                    _ => null
                },
                GREATER => (double)left! > (double)right!,
                GREATER_EQUAL => (double)left! >= (double)right!,
                LESS => (double)left! < (double)right!,
                LESS_EQUAL => (double)left! <= (double)right!,
                EQUAL_EQUAL => IsEqual(left, right),
                BANG_EQUAL => !IsEqual(left, right),
                _ => null
            };
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