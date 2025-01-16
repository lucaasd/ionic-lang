namespace VM.Runtime;

public record StackObjectRef(int Start, int End, Type Type) : Interval(Start, End);