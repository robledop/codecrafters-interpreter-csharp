using static LoxInterpreter.TokenType;

namespace LoxInterpreter.Parser;

// Grammar
//
// program        : statement* EOF ;
// statement      : exprStmt | printStmt ;
// exprStmt       : expression ";" ;
// printStmt      : "print" expression ";" ;
//
// expression     : equality ;
// equality       : comparison ( ( "!=" | "==" ) comparison )* ;
// comparison     : term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
// term           : factor ( ( "-" | "+" ) factor )* ;
// factor         : unary ( ( "/" | "*" ) unary )* ;
// unary          : ( "!" | "-" ) unary | primary ;
// primary        : NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" ;

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

    public List<IStmt>? Parse()
    {
        var statements = new List<IStmt>();
        while (!IsAtEnd())
        {
            statements.Add(Statement());
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

    IStmt Statement()
    {
        if (Match(PRINT)) return PrintStatement();
        return ExpressionStatement();
    }

    IStmt PrintStatement()
    {
        var value = Expression();
        Consume(SEMICOLON, "Expect ';' after value.");
        return new Print(value);
    }

    IStmt ExpressionStatement()
    {
        var expr = Expression();
        Consume(SEMICOLON, "Expect ';' after expression.");
        return new StmtExpression(expr);
    }

    // expression     : equality ;
    IExpr Expression() => Equality();

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

    // primary: NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" ;
    IExpr Primary()
    {
        if (Match(FALSE)) return new Literal(false);
        if (Match(TRUE)) return new Literal(true);
        if (Match(NIL)) return new Literal(null);

        if (Match(NUMBER, STRING))
        {
            return new Literal(Previous().Literal);
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

    void Consume(TokenType type, string message)
    {
        if (Check(type)) Advance();
        else throw Error(Peek(), message);
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