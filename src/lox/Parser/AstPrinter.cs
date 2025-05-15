using System.Text;

namespace LoxInterpreter.Parser;

public class AstPrinter : IExprVisitor<string>
{
    public string Print(IExpr expr) => expr.Accept(this);

    // public string Visit<TExpr>(TExpr expr) where TExpr : IExpr
    //     => expr switch
    //     {
    //         Assign { Name.Lexeme: not null } assign => Assign(assign.Name.Lexeme, assign.Value),
    //         Binary { Op.Lexeme: not null } binary =>
    //             Parenthesize(binary.Op.Lexeme, binary.Left, binary.Right),
    //         Grouping grouping => Parenthesize("group", grouping.Expr),
    //         Literal { Value: double d } literal when (double)literal.Value % 1 == 0 => d.ToString("F1"),
    //         Literal { Value: double d } => d.ToString("G"),
    //         Literal { Value: true } => "true",
    //         Literal { Value: false } => "false",
    //         Literal { Value: null } => "nil",
    //         Literal { Value: not null } literal => literal.Value.ToString()!,
    //         Unary { Op.Lexeme: not null } unary => Parenthesize(unary.Op.Lexeme, unary.Right),
    //         _ => throw new NotImplementedException($"Unknown expression type: {expr.GetType()}")
    //     };

    string Assign(string name, IExpr value)
    {
        var sb = new StringBuilder();
        sb.Append(" = ").Append(name);
        sb.Append(' ');
        sb.Append(value.Accept(this));
        return sb.ToString();
    }

    string Parenthesize(string name, params IExpr[] exprs)
    {
        var sb = new StringBuilder();
        sb.Append("(").Append(name);
        foreach (var expr in exprs)
        {
            sb.Append(" ");
            sb.Append(expr.Accept(this));
        }

        sb.Append(")");
        return sb.ToString();
    }

    public string VisitAssignExpression(Assign expr)
    {
        if (expr.Name.Lexeme is null) throw new NotImplementedException();
        return Assign(expr.Name.Lexeme, expr.Value);
    }

    public string VisitBinaryExpression(Binary expr)
    {
        if (expr.Op.Lexeme is null) throw new NotImplementedException();
        return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
    }

    public string VisitGroupingExpression(Grouping expr)
    {
        return Parenthesize("group", expr.Expression);
    }

    public string VisitLiteralExpression(Literal expr)
    {
        return expr.Value switch
        {
            double d when d % 1 == 0 => d.ToString("F1"),
            double d => d.ToString("G"),
            true => "true",
            false => "false",
            null => "nil",
            _ => expr.Value.ToString()!
        };
    }

    public string VisitUnaryExpression(Unary expr)
    {
        if (expr.Op.Lexeme is null) throw new NotImplementedException();
        return Parenthesize(expr.Op.Lexeme, expr.Right);
    }

    public string VisitVariableExpression(Variable expr)
    {
        throw new NotImplementedException();
    }

    public string VisitLogicalExpression(Logical expr)
    {
        throw new NotImplementedException();
    }

    public string VisitCallExpression(Call expr)
    {
        throw new NotImplementedException();
    }

    public string VisitGetExpression(Get expr)
    {
        throw new NotImplementedException();
    }

    public string VisitSetExpression(Set expr)
    {
        throw new NotImplementedException();
    }

    public string VisitThisExpression(This expr)
    {
        throw new NotImplementedException();
    }

    public string VisitSuperExpression(Super expr)
    {
        throw new NotImplementedException();
    }
}