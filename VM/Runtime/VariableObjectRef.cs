namespace VM.Runtime;

public record VariableObjectRef(int Index, int End) : Interval(Index, End);