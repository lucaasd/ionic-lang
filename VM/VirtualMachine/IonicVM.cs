using System.Text;
using VM.Instructions;
using VM.Runtime;
using System.Runtime.InteropServices;
using VM.Memory;
using VM.Extensions;
using VM.Configuration;

namespace VM.VirtualMachine;

public class IonicVM
{
    private List<IByteCodePart> code = [];
    private List<Function> functions = [];
    private Frame currentFrame;
    private readonly Frame mainFrame;
    private Function? currentFunction;

    private Dictionary<Instruction, Action<byte[]>> opcodeActionDictionary;
    private List<Type> primitiveTypes = [typeof(byte), typeof(short), typeof(int), typeof(long)];
    private int programCounter;
    private int currentTypeIndex;

    private Stream? stdoutStream;
    private Stream? stderrStream;
    private Stream? stdinStream;

    public int ProgramCounter => programCounter;
    public int CurrentTypeIndex { get => currentTypeIndex; set => currentTypeIndex = value; }
    public Frame CurrentFrame => currentFrame;
    public Stack<byte>? Stack => currentFrame?.Stack;

    public static IonicVM CreateVM(VMBuilder builder)
    {
        return new IonicVM()
        {
            stderrStream = builder.StderrStream,
            stdoutStream = builder.StdoutStream,
            stdinStream = builder.StdinStream
        };
    }

    public IonicVM()
    {
        mainFrame = new Frame("_");
        currentFrame = mainFrame;
        opcodeActionDictionary = new()
        {
            {Instruction.PUSH, ExecutePush},
            {Instruction.SUM, ExecuteSum},
            {Instruction.AS, ExecuteAs},
            {Instruction.STORE, ExecuteStore},
            {Instruction.WRITE_STD, ExecuteWriteToStd},
            {Instruction.GOTO, ExecuteGoto},
            {Instruction.NOP, ExecuteNop}
        };
    }

    public string ConvertCodeToHumanIL()
    {
        StringBuilder stringBuilder = new();
        foreach (var instruction in code)
        {
            stringBuilder.AppendLine(instruction.ToString());
        }
        return stringBuilder.ToString();
    }

    public void LoadCode(List<IByteCodePart> code)
    {
        this.code = code;
    }

    public void Run()
    {
        Run(code);
    }

    private void Run(List<IByteCodePart> code)
    {
        while (programCounter < code.Count)
        {
            RunCode(code[programCounter]);
        }
    }

    public void RunSingle()
    {
        RunCode(code[programCounter]);
    }

    private void RunCode(IByteCodePart part)
    {
        if (currentFunction is null)
        {
            if (part is Operation)
            {
                var operation = (part as Operation)!;
                Execute(operation.Instruction, operation.Operand);
            }
        }
    }

    private void Execute(Instruction instruction, byte[] operand)
    {
        opcodeActionDictionary[instruction](operand);
    }

    private void ExecutePush(byte[] operand)
    {
        foreach (var value in operand)
        {
            currentFrame.Stack.Push(value);
        }
        programCounter++;
    }

    private void ExecuteSum(byte[] _)
    {
        byte finalValue = 0;
        foreach (var value in currentFrame.Stack.ToArray())
        {
            finalValue += value;
        }
        currentFrame.Stack.Push(finalValue);
        programCounter++;
    }

    private void ExecuteAs(byte[] bytes)
    {
        try
        {
            int index = BitConverter.ToInt32(bytes);
            currentTypeIndex = index;
        }
        catch (Exception exception)
        {
            throw new Exception($"Failed to convert bytes to {typeof(int).FullName}. cause: {exception.Message}");
        }
        programCounter++;
    }

    private void ExecuteStore(byte[] bytes)
    {

        int index = BitConverter.ToInt32(bytes);
        int size = Marshal.SizeOf(primitiveTypes[currentTypeIndex]);

        if (currentFrame.Stack.Count != size)
        {
            throw new Exception("Value size and type size must be the same!");
        }

        var objRef = new VariableObjectRef(index, index + size);
        currentFrame.Variables.Add(new Variable(index, primitiveTypes[index]));
        foreach (var value in currentFrame.Stack.ToArray().Reverse())
        {
            currentFrame.VariableMemory.Add(value);
        }
        currentFrame.Stack.Pop(size);

        programCounter++;
    }

    private void ExecuteWriteToStd(byte[] bytes)
    {
        byte target = bytes[0];

        static void WriteToStdout(byte[] data, Stream output)
        {
            output.Write(data);
        }

        static void WriteToStderr(byte[] data, Stream output)
        {
            output.Write(data);
        }

        static void WriteToStdin(byte[] data, Stream output)
        {
            output.Write(data);
        }

        var data = currentFrame.Stack.ToArray();

        if (stdinStream is null)
        {
            throw new Exception("Stdin is null");
        }

        if (stdoutStream is null)
        {
            throw new Exception("Stdout is null");
        }

        if (stderrStream is null)
        {
            throw new Exception("Stderr is null");
        }

        Action[] targetFunctionArray = [
            () => {
                WriteToStdin([..currentFrame.Stack.Reverse()], stdinStream);
            },
            () => {
                WriteToStdout([..currentFrame.Stack.Reverse()], stdoutStream);

            },
            () => {
                WriteToStderr([..currentFrame.Stack.Reverse()], stderrStream);
            }
        ];

        targetFunctionArray[target]();
        int count = currentFrame.Stack.Count;
        currentFrame.Stack.Pop(count);
        programCounter++;
    }

    private void ExecuteGoto(byte[] bytes)
    {
        var target = BitConverter.ToInt32(bytes);
        programCounter = target;
    }

    private void ExecuteNop(byte[] _)
    {
        programCounter++;
    }
}