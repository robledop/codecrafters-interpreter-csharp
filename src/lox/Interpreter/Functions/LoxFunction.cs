using CSharpLox.Parser;

namespace CSharpLox.Interpreter.Functions;

public class LoxFunction(Function declaration, LoxEnvironment closure, bool isInitializer = false) : ICallable
{
    public object? Call(LoxInterpreter loxInterpreter, List<object> arguments)
    {
        var environment = new LoxEnvironment(closure);
        for (var i = 0; i < declaration.Parameters.Count; i++)
        {
            var lexeme = declaration.Parameters[i].Lexeme;
            environment.Define(lexeme!, arguments[i]);
        }

        try
        {
            loxInterpreter.ExecuteBlock(declaration.Body, environment);
        }
        catch (ReturnException ret)
        {
            return isInitializer
                ? closure.GetAt(0, new Token(TokenType.THIS, "this", null, -1))
                : ret.Value;
        }

        if (isInitializer)
            return closure.GetAt(0, new Token(TokenType.THIS, "this", null, -1));

        return null;
    }

    public int Arity() => declaration.Parameters.Count;

    public LoxFunction Bind(LoxInstance instance)
    {
        var environment = new LoxEnvironment(closure);
        environment.Define("this", instance);
        return new LoxFunction(declaration, environment, isInitializer);
    }

    public override string ToString() => $"<fn {declaration.Name.Lexeme}>";
}