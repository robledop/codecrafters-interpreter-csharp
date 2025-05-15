namespace CSharpLox.Interpreter.Functions;

public class Clock : ICallable
{
    public object Call(LoxInterpreter loxInterpreter, List<object> arguments)
    {
        if (arguments.Count != 0)
        {
            throw new RuntimeError(new Token(TokenType.CLOCK, "clock"), "clock() takes no arguments.");
        }

        return (double)DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    public int Arity()
    {
        return 0;
    }

    public override string ToString() => "<native fn>";
}