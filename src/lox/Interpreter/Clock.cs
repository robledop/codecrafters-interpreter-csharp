namespace LoxInterpreter.Interpreter;

public class Clock : ICallable
{
    public object Call(Interpreter interpreter, List<object> arguments)
    {
        if (arguments.Count != 0)
        {
            throw new RuntimeError(new Token(TokenType.CLOCK, "clock"), "clock() takes no arguments.");
        }

        return (double)DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;
    }

    public int Arity()
    {
        return 0;
    }

    public override string ToString() => "<native fn>";
}