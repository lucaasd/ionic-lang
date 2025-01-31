using LLVMSharp;

namespace VM.Code;

public class Lexer(string code)
{
    private readonly char[] rawCode = code.ToCharArray();
    private int index = 0;
    private void Advance() => index++;
    private char CurrentChar => rawCode[index];
    private List<Token> tokens = [];
    public Token[] GetTokens() => [.. tokens];

    public void Reset()
    {
        index = 0;
        tokens = [];
    }

    private static readonly Dictionary<string, TokenType> instructionDictionary = new()
    {
        {"PUSH", TokenType.PUSH},
        {"SUM", TokenType.SUM},
        {"AS", TokenType.AS},
        {"STORE", TokenType.STORE},
        {"WRITE_STD", TokenType.WRITE_STD},
        {"GOTO", TokenType.GOTO},
        {"NOP", TokenType.NOP}
    };

    public static Dictionary<string, TokenType> InstructionDictionary => instructionDictionary;

    private void Instruction()
    {
        string tokenValue = "";
        while (char.IsUpper(CurrentChar) || CurrentChar == '_' && CurrentChar != '\0')
        {
            tokenValue += CurrentChar;
            Advance();
        }
        if (instructionDictionary.TryGetValue(tokenValue, out TokenType value))
        {
            tokens.Add(new Token(tokenValue, value));
        }
    }

    private void Digit()
    {
        var tokenValue = "";
        while (char.IsDigit(CurrentChar) && CurrentChar != '\0')
        {
            tokenValue += CurrentChar;
            Advance();
        }

        tokens.Add(new Token(tokenValue, TokenType.Digit));
    }

    public void Tokenize()
    {
        while (CurrentChar != '\0')
        {
            if (char.IsLetter(CurrentChar) || CurrentChar == '_')
            {
                Instruction();
            }
            else if (char.IsDigit(CurrentChar))
            {
                Digit();
            }
            else if (char.IsWhiteSpace(CurrentChar) || CurrentChar == '\n')
            {
                Advance();
            }
            else
            {
                throw new Exception($"Unrecognized {CurrentChar}");
            }
        }
        tokens.Add(new Token("END", TokenType.END));
    }
}