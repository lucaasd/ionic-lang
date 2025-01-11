namespace VM.Memory;

public struct Variable(long id, Type type, long value)
{
    public long ID { get; } = id;
    public Type Type { get; } = type;
    public long Value { get; } = value;

    public override string ToString()
    {
        return $"Variable {{ ID: {ID}, Type: {Type}, Value: {Value} }}";
    }
}
