namespace VM.Memory;

public struct Variable(int id, Type type)
{
    public int ID { get; } = id;
    public Type Type { get; } = type;

    public override string ToString()
    {
        return $"Variable {{ ID: {ID}, Type: {Type} }}";
    }
}
