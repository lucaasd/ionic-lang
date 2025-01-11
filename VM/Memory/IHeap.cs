namespace VM.Memory;

public interface IHeap
{

    public byte this[long index]
    {
        get;
    }
}