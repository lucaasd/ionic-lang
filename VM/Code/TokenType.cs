namespace VM.Code;

public enum TokenType
{
    // Identifiers

    PUSH,
    NOP,
    SUM,
    GOTO,
    STORE,
    LOAD,
    RETURN,
    INVOKE,
    AS,
    WRITE_STD,

    // Types

    Digit,

    // Special

    END
}