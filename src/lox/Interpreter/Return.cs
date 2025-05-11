namespace LoxInterpreter.Interpreter;

public class Return : Exception
{
    public object? Value { get; }

    public Return(object? value)
    {
        Value = value;
    }
}