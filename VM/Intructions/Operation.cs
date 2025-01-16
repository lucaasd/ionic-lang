using VM.VirtualMachine;

namespace VM.Instructions;

public record Operation(Instruction Instruction, byte[] Operand, Type Type) : IByteCodePart
{
    public Operation(Instruction Instruction, Type Type) : this(Instruction, [], Type) { }
    public Operation(Instruction Instruction) : this(Instruction, [], typeof(byte)) { }

    public override string ToString()
    {
        return $"{Instruction} {Operand}";
    }
}