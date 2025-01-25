using System.ComponentModel;
using VM.Code;
using VM.Instructions;

[TestFixture]
public class ConverterTests
{
    [Test]
    public void TestSingleInstruction()
    {
        Token[] input = [
            new Token("SUM", TokenType.SUM),
            new Token("END", TokenType.END)
        ];

        var converter = new Converter(input);

        converter.Convert();
        var code = converter.ByteCode;

        IByteCodePart[] expected = [
            new Operation(Instruction.SUM, [])
        ];

        Assert.That(code, Is.EqualTo(expected));
    }

    [Test]
    public void TestInstructionWithArgument()
    {
        Token[] input = [
            new Token("PUSH", TokenType.PUSH),
            new Token("10", TokenType.Digit),
            new Token("END", TokenType.END)
        ];

        var converter = new Converter(input);

        converter.Convert();
        var code = converter.ByteCode;

        IByteCodePart[] expected = [
            new Operation(Instruction.PUSH, [10, 0, 0, 0, 0, 0, 0, 0])
        ];


        Assert.That((code[0] as Operation)?.Instruction, Is.EqualTo((expected[0] as Operation)?.Instruction));
        Assert.That((code[0] as Operation)?.Operand, Is.EqualTo((expected[0] as Operation)?.Operand));
    }
}