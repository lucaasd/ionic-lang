namespace VM.AssemblyInfo;

public class Descriptor(string[] argumentTypes, string returnType)
{
    public string[] ArgumentTypes => argumentTypes;
    public string ReturnType => returnType;

    public string[] argumentTypes = argumentTypes;
    public string returnType = returnType;
}