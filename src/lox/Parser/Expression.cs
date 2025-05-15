namespace CSharpLox.Parser;

public interface IExprVisitor<out TResult>
{
    TResult VisitAssignExpression(Assign expr);
    TResult VisitBinaryExpression(Binary expr);
    TResult VisitGroupingExpression(Grouping expr);
    TResult VisitLiteralExpression(Literal expr);
    TResult VisitUnaryExpression(Unary expr);
    TResult VisitVariableExpression(Variable expr);
    TResult VisitLogicalExpression(Logical expr);
    TResult VisitCallExpression(Call expr);
    TResult VisitGetExpression(Get expr);
    TResult VisitSetExpression(Set expr);
    TResult VisitThisExpression(This expr);
    TResult VisitSuperExpression(Super expr);
}

// Visitable interface for all expressions
public interface IExpr
{
    TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor);
}

public record Assign(Token Name, IExpr Value) : IExpr
{
    public TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor)
        => exprVisitor.VisitAssignExpression(this);
}

public record Binary(IExpr Left, Token Op, IExpr Right) : IExpr
{
    public TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor)
        => exprVisitor.VisitBinaryExpression(this);
}

public record Grouping(IExpr Expression) : IExpr
{
    public TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor)
        => exprVisitor.VisitGroupingExpression(this);
}

public record Literal(object? Value) : IExpr
{
    public TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor)
        => exprVisitor.VisitLiteralExpression(this);
}

public record Unary(Token Op, IExpr Right) : IExpr
{
    public TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor)
        => exprVisitor.VisitUnaryExpression(this);
}

public record Variable(Token Name) : IExpr
{
    public TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor)
        => exprVisitor.VisitVariableExpression(this);
}

public record Logical(IExpr Left, Token Op, IExpr Right) : IExpr
{
    public TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor)
        => exprVisitor.VisitLogicalExpression(this);
}

public record Call(IExpr Callee, Token Paren, List<IExpr> Arguments) : IExpr
{
    public TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor)
        => exprVisitor.VisitCallExpression(this);
}

public record Get(IExpr Object, Token Name) : IExpr
{
    public TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor)
        => exprVisitor.VisitGetExpression(this);
}

public record Set(IExpr Object, Token Name, IExpr Value) : IExpr
{
    public TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor)
        => exprVisitor.VisitSetExpression(this);
}

public record This(Token Keyword) : IExpr
{
    public TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor)
        => exprVisitor.VisitThisExpression(this);
}

public record Super(Token Keyword, Token Method) : IExpr
{
    public TResult Accept<TResult>(IExprVisitor<TResult> exprVisitor)
        => exprVisitor.VisitSuperExpression(this);
}