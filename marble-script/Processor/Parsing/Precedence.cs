namespace Marble.Processor.Parsing;

public enum Precedence
{
    LOWEST = 1,
    EQUALS,      // ==
    LESSGREATER, // >, <
    SUM,         // +
    PRODUCT,     // *
    PREFIX,      // -x, !x
    CALL,        // myFunction(x)
}