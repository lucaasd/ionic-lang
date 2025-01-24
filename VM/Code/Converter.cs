using VM.Instructions;

namespace VM.Code;

public class Converter(Token[] _tokens)
{
    private Token[] tokens = _tokens;
    private List<IByteCodePart> byteCode = [];
    public IByteCodePart[] ByteCode => [.. byteCode];

    private int index;
    private void Advance() => index++;
    private Token CurrentToken => tokens[index];

    public void Convert()
    {
        IByteCodePart? currentBytecodePart = null;

        while (CurrentToken.Type != TokenType.END)
        {
            if (Lexer.InstructionDictionary.TryGetValue(CurrentToken.Type.ToString(), out var tokenType) && Enum.TryParse<Instruction>(tokenType.ToString(), out var instruction))
            {
                currentBytecodePart = new Operation(instruction, [], typeof(byte[]));
                Advance();
                byteCode.Add(currentBytecodePart);
            }
            else if (long.TryParse(CurrentToken.Value, out var number) && currentBytecodePart is Operation)
            {
                var operation = currentBytecodePart as Operation;
                if (operation is not null)
                {
                    var oldOperand = operation.Operand;
                    operation.Operand = [.. oldOperand.Concat(BitConverter.GetBytes(number))];
                }
                Advance();
            }
        }
    }
}