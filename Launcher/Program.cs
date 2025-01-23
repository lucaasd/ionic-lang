using System.Text;
using VM.Configuration;
using VM.Instructions;
using VM.VirtualMachine;

var code = new List<IByteCodePart> {
    new Operation(Instruction.PUSH, Encoding.UTF8.GetBytes("Hello, World!\n"), typeof(byte[])),
    new Operation(Instruction.WRITE_STD, [0], typeof(byte[]))
};

var builder = new VMBuilder()
    .ConfigureStdinStream(Console.OpenStandardInput())
    .ConfigureStdoutStream(Console.OpenStandardOutput())
    .ConfigureStderrStream(Console.OpenStandardError());

var vm = IonicVM.CreateVM(builder);

vm.LoadCode(code);
vm.Run();
