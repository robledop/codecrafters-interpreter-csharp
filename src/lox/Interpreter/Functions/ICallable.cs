namespace CSharpLox.Interpreter.Functions;

public interface ICallable
{
    object? Call(LoxInterpreter loxInterpreter, List<object> arguments);
    int Arity();
}