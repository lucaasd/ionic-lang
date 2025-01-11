namespace VM.Extensions;

public static class StackExtensions
{
    public static T?[] Pop<T>(this Stack<T> stack, int count)
    {
        List<T?> values = [];
        for (int i = 0; i < count; i++)
        {
            if (stack.Count == 0)
            {
                values.Add(default);
            }
            else
            {
                values.Add(stack.Pop());

            }
        }
        return [.. values];
    }
}