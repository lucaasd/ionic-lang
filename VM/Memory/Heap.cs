using System.Text;
using VM.Memory;

public class Heap : IHeap
{
    private byte[] memory = [];
    private int count;

    public byte this[long index] => memory[index];

    public int Count => count;

    public void Add(byte item)
    {
        if (count < memory.Length)
        {
            var oldMemory = memory;
            memory = new byte[oldMemory.Length + 8];
            oldMemory.CopyTo(memory, 0);
            memory[oldMemory.Length] = item;
            count += 1;
        }
    }

    public void Remove(byte index)
    {
        memory[index] = 0;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        foreach (var value in memory)
        {
            stringBuilder.Append($"[{value}]");
        }
        return stringBuilder.ToString();
    }
}