using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Security.Permissions;
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

    [Test]
    public void TestAs()
    {
        var vm = new IonicVM();
        List<IByteCodePart> code = [
            new Operation(Instruction.AS, BitConverter.GetBytes(1), typeof(byte[]))
        ];

        vm.LoadCode(code);
        vm.Run();
        Assert.That(vm.CurrentTypeIndex, Is.EqualTo(1));
    }

    [Test]
    public void TestStore()
    {
        var vm = new IonicVM();
        List<IByteCodePart> code = [
            new Operation(Instruction.STORE, BitConverter.GetBytes(2), typeof(byte[]))
        ];

        vm.LoadCode(code);
        vm.Run();
        Assert.Multiple(() =>
        {
            Assert.That(vm.CurrentFrame.VariableMemory.Take(4).ToArray(), Is.EqualTo(BitConverter.GetBytes(2)));
            Assert.That(vm.CurrentFrame.Variables.Where((variable) => variable.ID == 2), Is.Not.Null);
            Assert.That(vm.CurrentFrame.Variables.Count, Is.EqualTo(1));
        });

    }
}