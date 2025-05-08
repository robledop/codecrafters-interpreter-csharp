namespace LoxInterpreter.Interpreter;

public class RuntimeError : Exception
{
    public RuntimeError(Token token, string message) : base(message)
    {
        Token = token;
    }

    public Token Token { get; }
}