using static CSharpLox.TokenType;

namespace CSharpLox.Parser;

public class Parser(List<Token> tokens)
{
    readonly Stack<object?> _loopStack = new();
    int _current;

    // Used in earlier versions of the code
    public IExpr? ParseExpression()
    {
        try
        {
            return Expression();
        }
        catch (ParseError)
        {
            return null;
        }
    }

    public List<IStmt> Parse()
    {
        var statements = new List<IStmt>();
        while (!IsAtEnd())
        {
            var statement = Declaration();
            if (statement != null) statements.Add(statement);
        }

        return statements;
    }

    Token Previous() => tokens[_current - 1];
    Token Peek() => tokens[_current];
    bool IsAtEnd() => Peek().Type == EOF;

    bool Match(params TokenType[] types)
    {
        if (!types.Any(Check)) return false;

        Advance();
        return true;
    }

    bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw Error(Peek(), message);
    }

    static ParseError Error(Token token, string message)
    {
        Lox.Report(token.Line, token.Column, token.Type == EOF ? " at end" : $" at '{token.Lexeme}'", message);
        return new ParseError(token, message);
    }

    void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type is SEMICOLON) return;
            if (Peek().Type is CLASS or FUN or VAR or FOR or IF or WHILE or PRINT or RETURN) return;

            Advance();
        }
    }

    // declaration    : varDecl | statement ;
    IStmt? Declaration()
    {
        try
        {
            if (Match(CLASS)) return ClassDeclaration();
            if (Match(FUN)) return Function("function");
            if (Match(VAR)) return VarDeclaration();
            return Statement();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }

    // classDecl      : "class" IDENTIFIER ( "<" IDENTIFIER )? "{" function* "}" ;
    IStmt ClassDeclaration()
    {
        var name = Consume(IDENTIFIER, "Expect class name.");

        Variable? superclass = null;
        if (Match(LESS))
        {
            Consume(IDENTIFIER, "Expect superclass name.");
            superclass = new Variable(Previous());
        }

        Consume(LEFT_BRACE, "Expect '{' before class body.");

        var methods = new List<Function>();
        while (!Check(RIGHT_BRACE) && !IsAtEnd())
        {
            methods.Add((Function)Function("method"));
        }

        Consume(RIGHT_BRACE, "Expect '}' after class body.");
        return new Class(name, superclass, methods);
    }

    // function       : "fun" IDENTIFIER "(" parameters? ")" block ;
    IStmt Function(string kind)
    {
        var name = Consume(IDENTIFIER, $"Expect {kind} name.");

        Consume(LEFT_PAREN, $"Expect '(' after {kind} name.");
        var parameters = new List<Token>();
        if (!Check(RIGHT_PAREN))
        {
            do
            {
                if (parameters.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 parameters.");
                }

                parameters.Add(Consume(IDENTIFIER, "Expect parameter name."));
            } while (Match(COMMA));
        }

        Consume(RIGHT_PAREN, "Expect ')' after parameters.");
        Consume(LEFT_BRACE, $"Expect '{{' before {kind} body.");

        var body = Block();
        return new Function(name, parameters, body);
    }

    // varDecl        : "var" IDENTIFIER ( "=" expression )? ";" ;
    IStmt VarDeclaration()
    {
        var name = Consume(IDENTIFIER, "Expect variable name.");

        IExpr? initializer = null;
        if (Match(EQUAL))
            initializer = Expression();

        Consume(SEMICOLON, "Expect ';' after variable declaration.");
        return new Var(name, initializer);
    }

    // statement      : exprStmt | printStmt | block ;
    IStmt Statement()
    {
        if (Match(FOR)) return ForStatement();
        if (Match(IF)) return IfStatement();
        if (Match(PRINT)) return PrintStatement();
        if (Match(RETURN)) return ReturnStatement();
        if (Match(WHILE)) return WhileStatement();
        if (Match(LEFT_BRACE)) return new Block(Block());
        if (Match(BREAK)) return BreakStatement();
        if (Match(CONTINUE)) return ContinueStatement();
        return ExpressionStatement();
    }

    // breakStmt      : "break" ";" ;
    IStmt BreakStatement()
    {
        var keyword = Previous();
        if (_loopStack.Count == 0)
        {
            Error(keyword, "break statement not inside a loop.");
        }

        Consume(SEMICOLON, "Expect ';' after break.");
        return new Break(keyword);
    }

    // continueStmt   : "continue" ";" ;
    IStmt ContinueStatement()
    {
        var keyword = Previous();
        if (_loopStack.Count == 0)
        {
            Error(keyword, "continue statement not inside a loop.");
        }

        Consume(SEMICOLON, "Expect ';' after continue.");
        return new Continue(keyword);
    }

    // returnStmt     : "return" expression? ";" ;
    IStmt ReturnStatement()
    {
        var keyword = Previous();
        IExpr? value = null;
        if (!Check(SEMICOLON)) value = Expression();

        Consume(SEMICOLON, "Expect ';' after return value.");
        return new ReturnStmt(keyword, value);
    }


    // forStmt        : "for" "(" ( varDecl | exprStmt | ";") expression? ";" expression? ")" statement ;
    IStmt ForStatement()
    {
        Consume(LEFT_PAREN, "Expect '(' after 'for'.");

        IStmt? initializer;
        if (Match(SEMICOLON)) initializer = null;
        else if (Match(VAR)) initializer = VarDeclaration();
        else initializer = ExpressionStatement();

        IExpr? condition = null;
        if (!Check(SEMICOLON)) condition = Expression();
        Consume(SEMICOLON, "Expect ';' after loop condition.");

        IExpr? increment = null;
        if (!Check(RIGHT_PAREN)) increment = Expression();
        Consume(RIGHT_PAREN, "Expect ')' after for clauses.");

        _loopStack.Push(null);
        var body = Statement();
        _loopStack.Pop();

        if (increment != null)
            body = new Block([body, new StmtExpression(increment)]);

        condition ??= new Literal(true);
        body = new While(condition, body);

        if (initializer != null)
            body = new Block([initializer, body]);

        return body;
    }

    // whileStmt      : "while" "(" expression ")" statement ;
    IStmt WhileStatement()
    {
        Consume(LEFT_PAREN, "Expect '(' after 'while'.");
        var condition = Expression();
        Consume(RIGHT_PAREN, "Expect ')' after condition.");

        _loopStack.Push(null);
        var body = Statement();
        _loopStack.Pop();

        return new While(condition, body);
    }

    // ifStmt         : "if" "(" expression ")" statement ( "else" statement )? ;
    IStmt IfStatement()
    {
        Consume(LEFT_PAREN, "Expect '(' after 'if'.");
        var condition = Expression();
        Consume(RIGHT_PAREN, "Expect ')' after condition.");

        var thenBranch = Statement();
        IStmt? elseBranch = null;
        if (Match(ELSE)) elseBranch = Statement();

        return new If(condition, thenBranch, elseBranch);
    }

    // block          : "{" declaration* "}" ;
    List<IStmt> Block()
    {
        var statements = new List<IStmt>();

        while (!Check(RIGHT_BRACE) && !IsAtEnd())
        {
            var declaration = Declaration();
            if (declaration != null)
            {
                statements.Add(declaration);
            }
        }

        Consume(RIGHT_BRACE, "Expect '}' after block.");

        return statements;
    }


    // printStmt      : "print" expression ";" ;
    IStmt PrintStatement()
    {
        var value = Expression();
        Consume(SEMICOLON, "Expect ';' after value.");
        return new Print(value);
    }


    // exprStmt       : expression ";" ;
    IStmt ExpressionStatement()
    {
        var expr = Expression();
        Consume(SEMICOLON, "Expect ';' after expression.");
        return new StmtExpression(expr);
    }

    // expression     : assignment ;
    IExpr Expression() => Assignment();


    // assignment: IDENTIFIER "=" assignment | equality ;
    IExpr Assignment()
    {
        var expr = Or();
        if (!Match(EQUAL)) return expr;

        var equals = Previous();
        var value = Assignment();

        return expr switch
        {
            Variable variable => new Assign(variable.Name, value),
            // Turning the Get into a Set allows we to assign to properties
            Get get => new Set(get.Object, get.Name, value),
            _ => throw Error(equals, "Invalid assignment target.")
        };
    }

    // or: and ( "or" and )* ;
    IExpr Or()
    {
        var expr = And();

        while (Match(OR))
        {
            var op = Previous();
            var right = And();
            expr = new Logical(expr, op, right);
        }

        return expr;
    }

    // and: equality ( "and" equality )* ;
    IExpr And()
    {
        var expr = Equality();

        while (Match(AND))
        {
            var op = Previous();
            var right = Equality();
            expr = new Logical(expr, op, right);
        }

        return expr;
    }

    // equality: comparison ( ( "!=" | "==" ) comparison )* ;
    IExpr Equality()
    {
        var expr = Comparison();

        while (Match(BANG_EQUAL, EQUAL_EQUAL))
        {
            var op = Previous();
            var right = Comparison();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    // comparison: term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
    IExpr Comparison()
    {
        var expr = Term();

        while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
        {
            var op = Previous();
            var right = Term();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    // term: factor ( ( "-" | "+" ) factor )* ;
    IExpr Term()
    {
        var expr = Factor();

        while (Match(MINUS, PLUS))
        {
            var op = Previous();
            var right = Factor();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    // factor: unary ( ( "/" | "*" ) unary )* ;
    IExpr Factor()
    {
        var expr = Unary();

        while (Match(SLASH, STAR))
        {
            var op = Previous();
            var right = Unary();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    // unary: ( "!" | "-" ) unary | primary ;
    IExpr Unary()
    {
        if (!Match(BANG, MINUS)) return Call();

        var op = Previous();
        var right = Unary();
        return new Unary(op, right);
    }

    // call: primary ( "(" arguments? ")" | "." IDENTIFIER )* ;
    IExpr Call()
    {
        var expr = Primary();

        while (true)
        {
            if (Match(LEFT_PAREN)) expr = FinishCall(expr);
            else if (Match(DOT))
            {
                var name = Consume(IDENTIFIER, "Expect property name after '.'.");
                expr = new Get(expr, name);
            }
            else break;
        }

        return expr;
    }

    IExpr FinishCall(IExpr callee)
    {
        var arguments = new List<IExpr>();

        if (!Check(RIGHT_PAREN))
        {
            do
            {
                if (arguments.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 arguments.");
                }

                arguments.Add(Expression());
            } while (Match(COMMA));
        }

        var paren = Consume(RIGHT_PAREN, "Expect ')' after arguments.");
        return new Call(callee, paren, arguments);
    }

    // primary: NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" |
    //                  IDENTIFIER | "this" | "super" "." IDENTIFIER ;
    IExpr Primary()
    {
        if (Match(FALSE)) return new Literal(false);
        if (Match(TRUE)) return new Literal(true);
        if (Match(NIL)) return new Literal(null);
        if (Match(NUMBER, STRING)) return new Literal(Previous().Literal);
        if (Match(THIS)) return new This(Previous());
        if (Match(IDENTIFIER)) return new Variable(Previous());
        if (Match(SUPER))
        {
            var keyword = Previous();
            Consume(DOT, "Expect '.' after 'super'.");
            var method = Consume(IDENTIFIER, "Expect superclass method name.");
            return new Super(keyword, method);
        }

        // ReSharper disable once InvertIf
        if (Match(LEFT_PAREN))
        {
            var expr = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after expression.");
            return new Grouping(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }
}

public class ParseError(Token token, string message) : Exception($"{token}: {message}")
{
}