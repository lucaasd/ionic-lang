using System.Security.Cryptography;
using VM.Instructions;
using VM.VirtualMachine;

namespace VMTests.VirtualMachine;

[TestFixture]
public class VMTest
{
    [Test]
    public void TestArrayPush()
    {
        var vm = new IonicVM();
        List<IByteCodePart> code = [
            new Operation(Instruction.PUSH, [1, 2, 3, 4], typeof(byte[]))
        ];
        vm.LoadCode(code);
        vm.Run();
        byte[] expected = [4, 3, 2, 1];
        Assert.That(vm.Stack?.ToArray(), Is.EqualTo(expected));
    }

    [Test]
    public void TestPush()
    {
        var vm = new IonicVM();
        List<IByteCodePart> code = [
            new Operation(Instruction.PUSH, [10], typeof(byte))
        ];
        vm.LoadCode(code);
        vm.Run();
        byte expected = 10;
        Assert.That(vm.Stack?.ToArray()[0], Is.EqualTo(expected));
    }

    [Test]
    public void TestSum()
    {
        var vm = new IonicVM();
        List<IByteCodePart> code = [
            new Operation(Instruction.PUSH, [1, 2, 3, 4], typeof(byte[])),
            new Operation(Instruction.SUM, typeof(byte))
        ];

        vm.LoadCode(code);
        vm.Run();
        byte expected = 10;
        Assert.That(vm.Stack?.ToArray()[0], Is.EqualTo(expected));
    }
}