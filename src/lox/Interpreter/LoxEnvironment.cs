namespace CSharpLox.Interpreter;

public class LoxEnvironment
{
    public LoxEnvironment? Enclosing { get; set; }
    readonly Dictionary<string, object?> _values = new();

    public LoxEnvironment(LoxEnvironment? enclosing = null) => Enclosing = enclosing;

    public void Define(string name, object? value) => _values[name] = value;

    public object? Get(Token token)
    {
        if (_values.TryGetValue(token.Lexeme!, out var value))
        {
            return value;
        }

        // This is in the book, but is it really necessary?
        // if (_enclosing != null)
        // {
        //     return _enclosing.Get(token);
        // }

        throw new RuntimeError(token, $"Undefined variable '{token.Lexeme}'.");
    }

    public object? GetAt(int distance, Token token) => Ancestor(distance, token).Get(token);

    public void Assign(Token name, object? value)
    {
        if (_values.ContainsKey(name.Lexeme!))
        {
            _values[name.Lexeme!] = value;
            return;
        }

        // This is in the book, but is it really necessary?
        // if (_enclosing != null)
        // {
        //     _enclosing.Assign(name, value);
        //     return;
        // }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void AssignAt(int distance, Token token, object? value) => Ancestor(distance, token).Assign(token, value);

    LoxEnvironment Ancestor(int distance, Token token)
    {
        var environment = this;
        for (var i = 0; i < distance; i++)
        {
            environment = environment.Enclosing ?? throw new RuntimeError(token, "Enclosing environment not found.");
        }

        return environment;
    }
}