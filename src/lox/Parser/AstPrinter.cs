using System.Text;

namespace LoxInterpreter.Parser;

public class AstPrinter : IVisitor<string>
{
    public string Print(IExpr expr) => expr.Accept(this);

    public string Visit<TExpr>(TExpr expr) where TExpr : IExpr
        => expr switch
        {
            Assign { Name.Lexeme: not null } assign => Assign(assign.Name.Lexeme, assign.Value),
            Binary { Op.Lexeme: not null } binary =>
                Parenthesize(binary.Op.Lexeme, binary.Left, binary.Right),
            Grouping grouping => Parenthesize("group", grouping.Expr),
            Literal { Value: double d } literal when (double)literal.Value % 1 == 0 => d.ToString("F1"),
            Literal { Value: double d } => d.ToString("G"),
            Literal literal => literal.Value?.ToString() ?? "nil",
            Unary { Op.Lexeme: not null } unary => Parenthesize(unary.Op.Lexeme, unary.Right),
            _ => throw new NotImplementedException($"Unknown expression type: {expr.GetType()}")
        };

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
}