using System;

namespace Reax.Lexer;

public static class Keywords
{
    public static char[] LET => new char[] {'l', 'e', 't'};

    public static char[] IF => new char[] {'i', 'f'};

    public static char[] ELSE => new char[] {'e', 'l', 's', 'e'};

    public static char[] ON => new char[] {'o', 'n'};

    public static char[] TRUE => new char[] {'t', 'r', 'u', 'e'};

    public static char[] FALSE => new char[] {'f', 'a', 'l', 's', 'e'};

    public static char[] FUN => new char[] {'f', 'u', 'n'};

    public static char[] RETURN => new char[] {'r', 'e', 't', 'u', 'r', 'n'};

    public static char[] WHEN => new char[] {'w', 'h', 'e', 'n'};

    public static char[] FOR => new char[] {'f', 'o', 'r'};
    
    public static char[] TO => new char[] {'t', 'o'};

    public static char[] WHILE => new char[] {'w', 'h', 'i', 'l', 'e'};

    public static char[] AND => new char[] {'a', 'n', 'd'};
    
    public static char[] OR => new char[] {'o', 'r'};
    
    public static char[] NOT => new char[] {'n', 'o', 't'};

    public static char[] IMPORT => new char[] {'i', 'm', 'p', 'o', 'r', 't'};

    public static TokenType IsKeyword(char[] chars)
    {
        if (LET.SequenceEqual(chars)) return TokenType.LET;
        if (IF.SequenceEqual(chars)) return TokenType.IF;
        if (ELSE.SequenceEqual(chars)) return TokenType.ELSE;
        if (ON.SequenceEqual(chars)) return TokenType.ON;
        if (TRUE.SequenceEqual(chars)) return TokenType.TRUE;
        if (FALSE.SequenceEqual(chars)) return TokenType.FALSE;
        if (FUN.SequenceEqual(chars)) return TokenType.FUNCTION;
        if (RETURN.SequenceEqual(chars)) return TokenType.RETURN;
        if (WHEN.SequenceEqual(chars)) return TokenType.WHEN;
        if (FOR.SequenceEqual(chars)) return TokenType.FOR;
        if (TO.SequenceEqual(chars)) return TokenType.TO;
        if (WHILE.SequenceEqual(chars)) return TokenType.WHILE;
        if (AND.SequenceEqual(chars)) return TokenType.AND;
        if (OR.SequenceEqual(chars)) return TokenType.OR;
        if (NOT.SequenceEqual(chars)) return TokenType.NOT;
        if (IMPORT.SequenceEqual(chars)) return TokenType.IMPORT;
        return TokenType.IDENTIFIER;
    }
    
}
