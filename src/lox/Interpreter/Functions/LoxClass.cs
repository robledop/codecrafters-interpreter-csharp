namespace CSharpLox.Interpreter.Functions;

public class LoxClass(string name, LoxClass? superClass, Dictionary<string, LoxFunction> methods) : ICallable
{
    public string Name { get; init; } = name;
    public Dictionary<string, LoxFunction> Methods { get; } = methods;

    public override string ToString() => Name;

    public object Call(LoxInterpreter loxInterpreter, List<object> arguments)
    {
        var instance = new LoxInstance(this);
        FindMethod("init")?.Bind(instance).Call(loxInterpreter, arguments);

        return instance;
    }

    public LoxFunction? FindMethod(string name) =>
        Methods.GetValueOrDefault(name) ?? superClass?.FindMethod(name);

    public int Arity() => FindMethod("init")?.Arity() ?? 0;
}