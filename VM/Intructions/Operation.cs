using VM.VirtualMachine;

namespace VM.Instructions;

public record Operation : IByteCodePart
{
    public Instruction Instruction { get; set; }
    public byte[] Operand { get; set; }
    public Type Type { get; set; }

    public Operation(Instruction instruction, byte[] operand, Type type)
    {
        Instruction = instruction;
        Operand = operand;
        Type = type;
    }
    public Operation(Instruction Instruction, Type Type) : this(Instruction, [], Type) { }
    public Operation(Instruction Instruction) : this(Instruction, [], typeof(byte)) { }

    public override string ToString()
    {
        return $"{Instruction} {string.Join(", ", Operand)} {Type}";
    }
}