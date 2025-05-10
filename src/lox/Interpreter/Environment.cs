namespace LoxInterpreter.Interpreter;

public class Environment
{
    readonly Dictionary<string, object?> _values = new();

    public void Define(string name, object? value)
    {
        _values[name] = value;
    }

    public object? Get(Token name)
    {
        if (name.Lexeme is null)
        {
            throw new RuntimeError(name, "Undefined variable 'null'.");
        }

        if (_values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Assign(Token name, object? value)
    {
        if (name.Lexeme is null)
            throw new RuntimeError(name, "Undefined variable 'null'.");

        if (!_values.ContainsKey(name.Lexeme))
            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");

        _values[name.Lexeme] = value;
    }
}