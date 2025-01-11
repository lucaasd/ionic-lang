using VM.VirtualMachine;

namespace VM.Instructions;

public record Operation(Instruction Instruction, byte[] Operand) : IByteCodePart
{
    public Operation(Instruction Instruction, Type Type) : this(Instruction) { }
    public Operation(Instruction Instruction) : this(Instruction, []) { }

    public override string ToString()
    {
        return $"{Instruction} {Operand}";
    }
}