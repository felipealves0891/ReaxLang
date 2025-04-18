using System;

namespace Reax.Lexer;

public static class Keywords
{
    public static char[] LET => ['l', 'e', 't'];

    public static char[] IF => ['i', 'f'];

    public static char[] ELSE => ['e', 'l', 's', 'e'];

    public static char[] ON => ['o', 'n'];

    public static char[] TRUE => ['t', 'r', 'u', 'e'];

    public static char[] FALSE => ['f', 'a', 'l', 's', 'e'];

    public static char[] FUN => ['f', 'u', 'n'];

    public static char[] RETURN => ['r', 'e', 't', 'u', 'r', 'n'];

    public static char[] WHEN => ['w', 'h', 'e', 'n'];

    public static char[] FOR => ['f', 'o', 'r'];
    
    public static char[] TO => ['t', 'o'];

    public static char[] WHILE => ['w', 'h', 'i', 'l', 'e'];

    public static char[] AND => ['a', 'n', 'd'];
    
    public static char[] OR => ['o', 'r'];
    
    public static char[] NOT => ['n', 'o', 't'];

    public static char[] IMPORT => ['i', 'm', 'p', 'o', 'r', 't'];
    
    public static char[] SCRIPT => ['s', 'c', 'r', 'i', 'p', 't'];

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
        if (SCRIPT.SequenceEqual(chars)) return TokenType.SCRIPT;
        return TokenType.IDENTIFIER;
    }
    
}
