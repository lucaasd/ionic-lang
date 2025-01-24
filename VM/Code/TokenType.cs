namespace VM.Code;

public enum TokenType
{
    // Identifiers

    PUSH,
    SUM,
    AS,
    STORE,
    WRITE_STD,
    NOP,

    // Types

    Digit,

    // Special

    END
}