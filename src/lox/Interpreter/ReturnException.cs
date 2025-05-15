namespace CSharpLox.Interpreter;

public class ReturnException(object? value) : Exception
{
    public object? Value { get; } = value;
}

public class ContinueException : Exception;

public class BreakException : Exception;