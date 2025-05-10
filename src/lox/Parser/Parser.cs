using static LoxInterpreter.TokenType;

namespace LoxInterpreter.Parser;

// Grammar
//
// program        : statement* EOF ;
// declaration    : varDecl | statement ;
// varDecl        : "var" IDENTIFIER ( "=" expression )? ";" ;
// statement      : exprStmt | printStmt | block ;
// block          : "{" declaration* "}" ;
// exprStmt       : expression ";" ;
// printStmt      : "print" expression ";" ;
// expression     : assignment ;
// assignment     : IDENTIFIER "=" assignment | equality ;
// equality       : comparison ( ( "!=" | "==" ) comparison )* ;
// comparison     : term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
// term           : factor ( ( "-" | "+" ) factor )* ;
// factor         : unary ( ( "/" | "*" ) unary )* ;
// unary          : ( "!" | "-" ) unary | primary ;
// primary        : NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" |
//                  IDENTIFIER | "this" | "super" "." IDENTIFIER ;

public class Parser(List<Token> tokens)
{
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

    public List<IStmt?> Parse()
    {
        var statements = new List<IStmt?>();
        while (!IsAtEnd())
        {
            statements.Add(Declaration());
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


    // declaration    : varDecl | statement ;
    IStmt? Declaration()
    {
        try
        {
            if (Match(VAR)) return VarDeclaration();
            return Statement();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }

    // varDecl        : "var" IDENTIFIER ( "=" expression )? ";" ;
    IStmt VarDeclaration()
    {
        var name = Consume(IDENTIFIER, "Expect variable name.");

        IExpr? initializer = null;
        if (Match(EQUAL))
        {
            initializer = Expression();
        }

        Consume(SEMICOLON, "Expect ';' after variable declaration.");
        return new Var(name, initializer);
    }

    // statement      : exprStmt | printStmt | block ;
    IStmt Statement()
    {
        if (Match(PRINT)) return PrintStatement();
        if (Match(LEFT_BRACE)) return new Block(Block());
        return ExpressionStatement();
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
        var expr = Equality();
        if (!Match(EQUAL)) return expr;

        var equals = Previous();
        var value = Assignment();

        if (expr is not Variable variable)
        {
            throw Error(equals, "Invalid assignment target.");
        }

        return new Assign(variable.Name, value);
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
        if (!Match(BANG, MINUS)) return Primary();

        var op = Previous();
        var right = Unary();
        return new Unary(op, right);
    }

    // primary: NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" |
    //                  IDENTIFIER | "this" | "super" "." IDENTIFIER ;
    IExpr Primary()
    {
        if (Match(FALSE)) return new Literal(false);
        if (Match(TRUE)) return new Literal(true);
        if (Match(NIL)) return new Literal(null);

        if (Match(NUMBER, STRING))
        {
            return new Literal(Previous().Literal);
        }

        if (Match(IDENTIFIER))
        {
            return new Variable(Previous());
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

    Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw Error(Peek(), message);
    }

    static ParseError Error(Token token, string message)
    {
        Lox.Report(token.Line, token.Type == EOF ? " at end" : $" at '{token.Lexeme}'", message);
        return new ParseError(token, message);
    }

    void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == SEMICOLON) return;
            if (Peek().Type is CLASS or FUN or VAR or FOR or IF or WHILE or PRINT or RETURN) return;

            Advance();
        }
    }
}

public class ParseError(Token token, string message) : Exception($"{token}: {message}")
{
}