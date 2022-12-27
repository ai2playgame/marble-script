namespace Marble.Processor.Parsing;

public enum Precedence
{
    Lowest = 1,
    EqualTwo,    // ==
    LessGreater, // >, <
    Sum,         // +
    Product,     // *
    Prefix,      // -x, !x
    Call,        // myFunction(x)
}