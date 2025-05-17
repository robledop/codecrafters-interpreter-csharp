namespace CSharpLox.Parser;

public interface IStmtVisitor<out TResult>
{
    TResult VisitBlockStatement(Block expr);
    TResult VisitClassStatement(Class stmt);
    TResult VisitExpressionStatement(StmtExpression expr);
    TResult VisitFunctionStatement(Function stmt);
    TResult VisitIfStatement(If expr);
    TResult VisitPrintStatement(Print expr);
    TResult VisitReturnStatement(ReturnStmt stmt);
    TResult VisitVarStatement(Var stmt);
    TResult VisitWhileStatement(While stmt);
    TResult VisitBreakStatement(Break stmt);
    TResult VisitContinueStatement(Continue stmt);
}

// Visitable interface for all statements
public interface IStmt
{
    TResult Accept<TResult>(IStmtVisitor<TResult> visitor);
}

public record Block(List<IStmt> Statements) : IStmt
{
    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor)
        => visitor.VisitBlockStatement(this);
}

public record Class(Token Name, Variable? SuperClass, List<Function> Methods) : IStmt
{
    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor)
        => visitor.VisitClassStatement(this);
}

public record StmtExpression(IExpr Expression) : IStmt
{
    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor)
        => visitor.VisitExpressionStatement(this);
}

public record Function(Token Name, List<Token> Parameters, List<IStmt> Body) : IStmt
{
    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor)
        => visitor.VisitFunctionStatement(this);
}

public record If(IExpr Condition, IStmt ThenBranch, IStmt? ElseBranch) : IStmt
{
    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor)
        => visitor.VisitIfStatement(this);
}

public record Print(IExpr Expression) : IStmt
{
    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor)
        => visitor.VisitPrintStatement(this);
}

public record ReturnStmt(Token Keyword, IExpr? Value) : IStmt
{
    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor)
        => visitor.VisitReturnStatement(this);
}

public record Var(Token Name, IExpr? Initializer) : IStmt
{
    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor)
        => visitor.VisitVarStatement(this);
}

public record While(IExpr Condition, IStmt Body) : IStmt
{
    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor)
        => visitor.VisitWhileStatement(this);
}

public record Break(Token Keyword) : IStmt
{
    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor)
        => visitor.VisitBreakStatement(this);
}

public record Continue(Token Keyword) : IStmt
{
    public TResult Accept<TResult>(IStmtVisitor<TResult> visitor)
        => visitor.VisitContinueStatement(this);
}