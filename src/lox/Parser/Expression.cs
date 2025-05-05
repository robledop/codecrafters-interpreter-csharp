namespace LoxInterpreter.Parser;

public interface IVisitor<out TResult>
{
    TResult Visit<TExpr>(TExpr expr) where TExpr : IExpr;
}

// Visitable interface for all expressions
public interface IExpr
{
    TResult Accept<TResult>(IVisitor<TResult> visitor) => visitor.Visit(this);
}

public record Assign(Token Name, IExpr Value) : IExpr;

public record Binary(IExpr Left, Token Op, IExpr Right) : IExpr;

public record Grouping(IExpr Expr) : IExpr;

public record Literal(object? Value) : IExpr;

public record Unary(Token Op, IExpr Right) : IExpr;

public record Variable(Token Name) : IExpr;

public record Logical(IExpr Left, Token Op, IExpr Right) : IExpr;

public record Call(IExpr Callee, Token Paren, List<IExpr> Arguments) : IExpr;

public record Get(IExpr Object, Token Name) : IExpr;

public record Set(IExpr Object, Token Name, IExpr Value) : IExpr;

public record This(Token Keyword) : IExpr;

public record Super(Token Keyword, Token Method) : IExpr;