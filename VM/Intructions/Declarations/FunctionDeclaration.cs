namespace VM.Instructions.Declarations;

public record FunctionDeclaration(string Name, string Descriptor) : IByteCodePart
{
    public override string ToString()
    {
        return $"{Name}{Descriptor}";
    }
}