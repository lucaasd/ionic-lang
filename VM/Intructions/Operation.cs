using VM.VirtualMachine;

namespace VM.Instructions;

public record Operation : IByteCodePart
{
    public Instruction Instruction { get; set; }
    public byte[] Operand { get; set; }

    public Operation(Instruction instruction, byte[] operand)
    {
        Instruction = instruction;
        Operand = operand;
    }

    public override string ToString()
    {
        return $"{Instruction} {string.Join(", ", Operand)}";
    }
}