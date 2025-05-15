using System.Text;

namespace CSharpLox.Parser;

public class AstPrinter : IExprVisitor<string>
{
    public string Print(IExpr expr) => expr.Accept(this);

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