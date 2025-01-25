namespace VM.Code;

public record Token(string Value, TokenType Type)
{
    public override string ToString()
    {
        return $"value: {Value} type {Type}";
    }
};