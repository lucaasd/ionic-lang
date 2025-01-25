using System.Text;
using VM.Instructions;
using VM.Instructions.Declarations;
using VM.AssemblyInfo.Validation;
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

    public int CurrentTypeIndex { get => currentTypeIndex; set => currentTypeIndex = value; }
    public Frame CurrentFrame => currentFrame;

    private Stream stdoutStream;
    private Stream stderrStream;
    private Stream stdinStream;

    public Stack<byte>? Stack => currentFrame?.Stack;

    public static IonicVM CreateVM(VMBuilder builder)
    {
        return new IonicVM()
        {
            stderrStream = builder.StderrStream,
            stdoutStream = builder.StdoutStream
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
            {Instruction.WRITE_STD, ExecuteWriteToStd}
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
            if (currentFunction is null)
            {
                if (code[programCounter] is Operation)
                {
                    var operation = (code[programCounter] as Operation)!;

                    var instruction = operation.Instruction;
                    var operand = operation?.Operand;


                    Execute(instruction, operand ?? []);
                }
                else if (code[programCounter] is FunctionDeclaration && currentFunction is null)
                {
                    var functionDeclaration = (code[programCounter] as FunctionDeclaration)!;

                    var descriptor = functionDeclaration.Descriptor;

                    var parsedDescriptor = DescriptorParser.Parse(descriptor);

                    if (parsedDescriptor is not null)
                    {
                        currentFunction = new Function(functionDeclaration.Name, parsedDescriptor);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Descriptor {descriptor} is not valid");
                    }

                    programCounter++;
                }
                else if (code[programCounter] is Operation && currentFunction is not null)
                {
                    var operation = (code[programCounter] as Operation)!;

                    if (operation.Instruction == Instruction.END)
                    {
                        functions.Add(currentFunction);
                        currentFunction = null;
                        programCounter++;
                        continue;
                    }

                    currentFunction.Code.Add(operation);
                    programCounter++;
                }
                else
                {
                    throw new InvalidOperationException($"Instruction or Declaration {code[programCounter].GetType().Name} not exists");
                }
            }
            else
            {
                if (code[programCounter] is Operation)
                {
                    var operation = (code[programCounter] as Operation)!;

                    currentFunction?.Code.Add(operation);
                }
                else
                {
                    throw new InvalidOperationException($"Instruction {code[programCounter].GetType().Name} not exists");
                }
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
}