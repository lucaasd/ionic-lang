using VM.Code;

[TestFixture]
public class LexerTest()
{
    [Test]
    public void TestInstructionWithoutOperand()
    {
        var input = "NOP\0";
        var lexer = new Lexer(input);
        lexer.Tokenize();
        var tokens = lexer.GetTokens();
        Assert.That(tokens[0].Type, Is.EqualTo(TokenType.NOP));
    }

    [Test]
    public void TestInstruction()
    {
        var input = "PUSH 10\0";
        var lexer = new Lexer(input);
        lexer.Tokenize();
        var tokens = lexer.GetTokens();
        var expectedInstructionToken = new Token("PUSH", TokenType.PUSH);
        var expectedOperandToken = new Token("10", TokenType.Digit);

        Assert.Multiple(() =>
        {
            Assert.That(tokens[0], Is.EqualTo(expectedInstructionToken));
            Assert.That(tokens[1], Is.EqualTo(expectedOperandToken));
        });

    }

    [Test]
    public void TestMultipleOperands()
    {
        var input = "PUSH 1 2\0";
        var lexer = new Lexer(input);
        lexer.Tokenize();
        var tokens = lexer.GetTokens();
        var expectedInstructionToken = new Token("PUSH", TokenType.PUSH);
        var expectedOperand1 = new Token("1", TokenType.Digit);
        var expectedOperand2 = new Token("2", TokenType.Digit);
        var end = new Token("END", TokenType.END);

        Token[] expected = [expectedInstructionToken, expectedOperand1, expectedOperand2, end];

        Assert.That(tokens, Is.EqualTo(expected));
    }

    [Test]
    public void TestMultipleInstructions()
    {
        var input = "PUSH 1 2\n SUM\0";
        var lexer = new Lexer(input);
        lexer.Tokenize();
        var tokens = lexer.GetTokens();
        var expectedInstructionToken1 = new Token("PUSH", TokenType.PUSH);
        var expectedOperand1 = new Token("1", TokenType.Digit);
        var expectedOperand2 = new Token("2", TokenType.Digit);
        var expectedInstructionToken2 = new Token("SUM", TokenType.SUM);
        var end = new Token("END", TokenType.END);


        Token[] expected = [expectedInstructionToken1, expectedOperand1, expectedOperand2, expectedInstructionToken2, end];
        Assert.That(tokens, Is.EqualTo(expected));
    }
}