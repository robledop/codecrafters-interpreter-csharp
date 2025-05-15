using CSharpLox.Parser;

namespace CSharpLox.Interpreter;

public record Resolver(LoxInterpreter LoxInterpreter) : IExprVisitor<object?>, IStmtVisitor<object?>
{
    enum FunctionType
    {
        NONE,
        FUNCTION,
        METHOD,
        INITIALIZER,
    }

    enum ClassType
    {
        NONE,
        CLASS,
        SUBCLASS,
    }

    readonly Stack<Dictionary<string, bool>> _scopes = new();
    FunctionType _currentFunction = FunctionType.NONE;

    public object? VisitAssignExpression(Assign expr)
    {
        Resolve(expr.Value);
        ResolveLocal(expr, expr.Name);
        return null;
    }

    public object? VisitBinaryExpression(Binary expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }

    public object? VisitCallExpression(Call expr)
    {
        Resolve(expr.Callee);
        expr.Arguments.ForEach(Resolve);

        return null;
    }

    public object? VisitGetExpression(Get expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitGroupingExpression(Grouping expr)
    {
        Resolve(expr.Expression);
        return null;
    }


    public object? VisitLiteralExpression(Literal expr) => null;

    public object? VisitLogicalExpression(Logical expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }

    public object? VisitSetExpression(Set expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitSuperExpression(Super expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitThisExpression(This expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitUnaryExpression(Unary expr)
    {
        Resolve(expr.Right);
        return null;
    }

    public object? VisitVariableExpression(Variable expr)
    {
        if (_scopes.Any() && _scopes.Peek().TryGetValue(expr.Name.Lexeme!, out var isDefined))
        {
            if (!isDefined)
                Lox.Error(expr.Name, "Can't read local variable in its own initializer.");
        }

        ResolveLocal(expr, expr.Name);
        return null;
    }

    public object? VisitBlockStatement(Block expr)
    {
        BeginScope();
        Resolve(expr.Statements);
        EndScope();
        return null;
    }

    public object? VisitClassStatement(Class expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitExpressionStatement(StmtExpression stmt)
    {
        Resolve(stmt.Expression);
        return null;
    }

    public object? VisitFunctionStatement(Function stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);

        ResolveFunction(stmt, FunctionType.FUNCTION);
        return null;
    }

    public object? VisitIfStatement(If expr)
    {
        Resolve(expr.Condition);
        Resolve(expr.ThenBranch);
        if (expr.ElseBranch is not null)
            Resolve(expr.ElseBranch);

        return null;
    }

    public object? VisitPrintStatement(Print expr)
    {
        Resolve(expr.Expression);
        return null;
    }

    public object? VisitReturnStatement(ReturnStmt stmt)
    {
        if (_currentFunction == FunctionType.NONE)
            throw new RuntimeError(stmt.Keyword, "Cannot return from top-level code.");

        if (_currentFunction == FunctionType.INITIALIZER && stmt.Value is not null)
            throw new RuntimeError(stmt.Keyword, "Cannot return a value from an initializer.");

        if (stmt.Value is not null)
            Resolve(stmt.Value);

        return null;
    }

    public object? VisitBreakStatement(Break stmt) => null;

    public object? VisitContinueStatement(Continue stmt) => null;

    public object? VisitVarStatement(Var stmt)
    {
        Declare(stmt.Name);
        if (stmt.Initializer is not null)
            Resolve(stmt.Initializer);

        Define(stmt.Name);
        return null;
    }

    public object? VisitWhileStatement(While stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.Body);
        return null;
    }

    void ResolveFunction(Function function, FunctionType functionType)
    {
        var enclosingFunction = _currentFunction;
        _currentFunction = functionType;

        BeginScope();
        foreach (var param in function.Parameters)
        {
            Declare(param);
            Define(param);
        }

        Resolve(function.Body);
        EndScope();

        _currentFunction = enclosingFunction;
    }

    void ResolveLocal(IExpr expr, Token name)
    {
        // Java stores the Stack elements in reverse order compared to C#.
        // So, this is different from the book.
        for (int i = 0; i < _scopes.Count; i++)
        {
            if (_scopes.ElementAt(i).ContainsKey(name.Lexeme!))
            {
                LoxInterpreter.Resolve(expr, i);
                return;
            }
        }
    }

    public void Resolve(List<IStmt> statements) => statements.ForEach(Resolve);
    void Resolve(IStmt stmt) => stmt.Accept(this);
    void Resolve(IExpr expr) => expr.Accept(this);
    void BeginScope() => _scopes.Push(new Dictionary<string, bool>());
    void EndScope() => _scopes.Pop();

    void Declare(Token name)
    {
        if (!_scopes.Any()) return;
        var scope = _scopes.Peek();
        if (scope.ContainsKey(name.Lexeme!))
            throw new RuntimeError(name, $"Variable '{name.Lexeme}' already declared in this scope.");

        scope.Add(name.Lexeme!, false);
    }

    void Define(Token name)
    {
        if (!_scopes.Any()) return;
        var scope = _scopes.Peek();
        scope[name.Lexeme!] = true;
    }
}