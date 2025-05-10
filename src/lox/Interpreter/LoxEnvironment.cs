namespace LoxInterpreter.Interpreter;

public class LoxEnvironment
{
    readonly LoxEnvironment? _enclosing;
    readonly Dictionary<string, object?> _values = new();

    public LoxEnvironment(LoxEnvironment? enclosing = null)
    {
        _enclosing = enclosing;
    }

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

        if (_enclosing != null)
        {
            return _enclosing.Get(name);
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Assign(Token name, object? value)
    {
        if (name.Lexeme is null)
            throw new RuntimeError(name, "Undefined variable 'null'.");

        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }

        if (_enclosing is not null)
        {
            _enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
}