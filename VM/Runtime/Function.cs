using VM.AssemblyInfo;
using VM.Instructions;

namespace VM.VirtualMachine.Runtime;

public class Function(string name, Descriptor descriptor)
{
    private List<IByteCodePart> code = [];

    public string Name => name;
    public Descriptor Descriptor => descriptor;
    public List<IByteCodePart> Code => code;
}
