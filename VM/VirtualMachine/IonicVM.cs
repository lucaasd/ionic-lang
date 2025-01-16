using System.Text;
using VM.Instructions;
using VM.Instructions.Declarations;
using VM.AssemblyInfo.Validation;
using VM.Runtime;

namespace VM.VirtualMachine;

public class IonicVM
{
    private List<IByteCodePart> code = [];
    private List<Function> functions = [];
    private Frame currentFrame;
    private readonly Frame mainFrame;
    private Function? currentFunction;

    private Dictionary<Instruction, Action<byte[]>> opcodeActionDictionary;
    private int programCounter;

    public Stack<byte>? Stack => currentFrame?.Stack;

    public IonicVM()
    {
        mainFrame = new Frame("_");
        currentFrame = mainFrame;
        opcodeActionDictionary = new()
        {
            {Instruction.PUSH, ExecutePush}
        };
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
}