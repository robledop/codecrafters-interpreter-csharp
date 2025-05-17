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
    ClassType _currentClass = ClassType.NONE;

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
        Resolve(expr.Object);
        return null;
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
        Resolve(expr.Value);
        Resolve(expr.Object);
        return null;
    }

    public object? VisitSuperExpression(Super expr)
    {
        if (_currentClass == ClassType.NONE)
            Lox.Error(expr.Keyword, "Can't use 'super' outside of a class.");

        if (_currentClass != ClassType.SUBCLASS)
            Lox.Error(expr.Keyword, "Can't use 'super' in a class with no superclass.");

        ResolveLocal(expr, expr.Keyword);
        return null;
    }

    public object? VisitThisExpression(This expr)
    {
        if (_currentClass == ClassType.NONE)
            Lox.Error(expr.Keyword, "Can't use 'this' outside of a class.");

        ResolveLocal(expr, expr.Keyword);
        return null;
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

    public object? VisitClassStatement(Class stmt)
    {
        var enclosingClass = _currentClass;
        _currentClass = ClassType.CLASS;

        Declare(stmt.Name);
        Define(stmt.Name);

        if (stmt.SuperClass is not null && stmt.Name.Lexeme == stmt.SuperClass.Name.Lexeme)
            Lox.Error(stmt.SuperClass.Name, "A class can't inherit from itself.");

        if (stmt.SuperClass is not null)
            Resolve(stmt.SuperClass);

        if (stmt.SuperClass is not null)
        {
            _currentClass = ClassType.SUBCLASS;
            BeginScope();
            _scopes.Peek().Add("super", true);
        }


        BeginScope();

        _scopes.Peek().Add("this", true);

        foreach (var method in stmt.Methods)
        {
            var declaration = FunctionType.METHOD;
            if (method.Name.Lexeme == "init")
                declaration = FunctionType.INITIALIZER;
            ResolveFunction(method, declaration);
        }

        EndScope();

        if (stmt.SuperClass is not null) EndScope();

        _currentClass = enclosingClass;

        return null;
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
            Lox.Error(stmt.Keyword, "Can't return from top-level code.");

        if (_currentFunction == FunctionType.INITIALIZER && stmt.Value is not null)
            Lox.Error(stmt.Keyword, "Cannot return a value from an initializer.");

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
            Lox.Error(name, "Already a variable with this name in this scope.");
        else
            scope.Add(name.Lexeme!, false);
    }

    void Define(Token name)
    {
        if (!_scopes.Any()) return;
        var scope = _scopes.Peek();
        scope[name.Lexeme!] = true;
    }
}