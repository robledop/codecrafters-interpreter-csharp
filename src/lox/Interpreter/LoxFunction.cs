using LoxInterpreter.Parser;

namespace LoxInterpreter.Interpreter;

public class LoxFunction(Function declaration, LoxEnvironment closure) : ICallable
{
    public object? Call(Interpreter interpreter, List<object> arguments)
    {
        var environment = new LoxEnvironment(closure);
        for (var i = 0; i < declaration.Parameters.Count; i++)
        {
            var lexeme = declaration.Parameters[i].Lexeme;
            environment.Define(lexeme!, arguments[i]);
        }

        try
        {
            interpreter.ExecuteBlock(declaration.Body, environment);
        }
        catch (ReturnException ret)
        {
            return ret.Value;
        }

        return null;
    }

    public int Arity() => declaration.Parameters.Count;

    public override string ToString() => $"<fn {declaration.Name.Lexeme}>";
}