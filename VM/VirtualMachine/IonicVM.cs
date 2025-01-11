using System.Text;
using VM.Instructions;
using VM.Extensions;
using VM.Instructions.Declarations;
using VM.Memory;
using VM.AssemblyInfo.Validation;
using VM.VirtualMachine.Runtime;
namespace VM.VirtualMachine;

public class IonicVM
{
    public static bool IsLong(byte[] bytes) => bytes.Length == 8;
    public static bool IsInteger(byte[] bytes) => bytes.Length == 4;
    public static bool IsByte(byte[] bytes) => bytes.Length == 1;

    private List<IByteCodePart> code = [];
    private List<Function> functions = [];
    private Frame currentFrame;
    private readonly Frame mainFrame;
    private Function? currentFunction;

    private int programCounter;

    public Stack<byte>? Stack => currentFrame?.Stack;

    public IonicVM()
    {
        mainFrame = new Frame("_");
        currentFrame = mainFrame;
    }

    public string ConvertCodeToHumanIL()
    {
        StringBuilder stringBuilder = new StringBuilder();
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
        Console.WriteLine("Running code");
        while (programCounter < code.Count)
        {
            if (currentFunction is null)
            {
                if (code[programCounter] is Operation)
                {
                    var operation = (code[programCounter] as Operation)!;

                    var instruction = operation.Instruction;
                    var operand = operation.Operand;

                    if (IsByte(operand))
                    {
                        Execute(instruction, operand[0]);
                    }
                    else if (IsInteger(operand))
                    {
                        Execute(instruction, BitConverter.ToInt32(operand));
                    }
                    else if (IsLong(operand))
                    {
                        Execute(instruction, BitConverter.ToInt64(operand));
                    }
                    else
                    {
                        Execute(instruction, operand);
                    }
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

                    Console.WriteLine($"Function {functionDeclaration.Name} declared");
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
        if (instruction == Instruction.PRINT)
        {
            Console.WriteLine(Encoding.UTF8.GetString([.. currentFrame.Stack.Reverse()]));
            programCounter++;
        }
        else if (instruction == Instruction.PUSH)
        {
            foreach (var _byte in operand)
            {
                currentFrame.Stack.Push(_byte);
            }
            programCounter++;
        }
    }

    /// <summary>
    /// Used for 8 bit instructions
    /// </summary>
    /// <param name="instruction"></param>
    /// <param name="operand"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void Execute(Instruction instruction, byte operand)
    {
        if (operand == 0)
        {
            Console.WriteLine($"{instruction}");
        }
        else
        {
            Console.WriteLine($"{instruction} {operand}");
        }

        if (instruction == Instruction.POP)
        {
            currentFrame.Stack.Pop();
            programCounter++;
        }
        else if (instruction == Instruction.NOP)
        {
            programCounter++;
        }
        else if (instruction == Instruction.SUM)
        {
            if (currentFrame.Stack.Count >= 2)
            {
                byte result = 0;

                for (int i = 0; i < currentFrame.Stack.Count; i++)
                {
                    var value = currentFrame.Stack.Pop();
                    result = (byte)(result + value);
                }
                currentFrame.Stack.Push(result);
                Console.WriteLine($"Result: {result}");
            }
            programCounter++;
        }
        else if (instruction == Instruction.RETURN)
        {
            var oldFrameStack = currentFrame.Stack;
            var parentFrame = currentFrame.Parent;
            currentFrame = parentFrame ?? mainFrame;
            _ = currentFrame.Stack.Append(oldFrameStack.Pop());
            programCounter++;
        }
        else if (instruction == Instruction.LOAD)
        {
            var bytes = BitConverter.GetBytes(currentFrame.Variables[operand].Value);
            foreach (var @byte in bytes)
            {
                currentFrame.Stack.Push(@byte);
            }
            programCounter++;
        }
        else
        {
            throw new InvalidOperationException($"Instruction {instruction} not exists");
        }
    }

    /// <summary>
    /// Used for 64 bits instructions
    /// </summary>
    /// <param name="instruction"></param>
    /// <param name="operand"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void Execute(Instruction instruction, long operand)
    {

    }

    /// <summary>
    /// Used for 32 bits instructions
    /// </summary>
    /// <param name="instruction"></param>
    /// <param name="operand"></param>
    private void Execute(Instruction instruction, int operand)
    {
        switch (instruction)
        {
            case Instruction.GOTO:
                programCounter = operand;
                break;
            case Instruction.ISTORE:
                Console.WriteLine($"Storing at {operand}");
                if (operand >= currentFrame.Variables.Length)
                {
                    var oldVariableArray = currentFrame.Variables;
                    var size = currentFrame.Variables.Length + 1;
                    currentFrame.Variables = new Variable[size + 16];
                    oldVariableArray.CopyTo(currentFrame.Variables, 0);
                    Console.WriteLine($"> Generating new array and copying data");
                }
                var bytes = currentFrame.Stack.Pop(4);
                currentFrame.Variables[operand] = new Variable(operand, typeof(long), BitConverter.ToInt32(bytes));
                programCounter++;
                break;
            case Instruction.INVOKE:
                var function = functions[operand];
                Console.WriteLine($"Invoking {function.Name}");
                currentFrame = new Frame(function.Name, currentFrame);
                Run(function.Code);
                programCounter++;
                break;
            default:
                throw new InvalidOperationException($"Instruction {instruction} not exists");

        }

    }
}