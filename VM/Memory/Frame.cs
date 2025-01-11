using VM.Memory;

public class Frame
{
    private Stack<byte> stack = [];
    private Variable[] variables = new Variable[16];
    private Frame? parent;
    private string name = "";

    public Stack<byte> Stack { get => stack; set { stack = value; } }
    public Variable[] Variables { get => variables; set => variables = value; }
    public Frame? Parent => parent;

    public Frame(string name, Frame? parent = null)
    {
        this.parent = parent;
    }
}