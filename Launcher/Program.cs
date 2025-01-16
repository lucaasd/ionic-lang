using VM.Instructions;
using VM.VirtualMachine;

var code = new List<IByteCodePart> {
    new Operation(Instruction.PUSH, [0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x57, 0x6f, 0x72, 0x6c, 0x64], typeof(byte[])),
    new Operation(Instruction.PRINT)
};

var vm = new IonicVM();
vm.LoadCode(code);
vm.Run();
