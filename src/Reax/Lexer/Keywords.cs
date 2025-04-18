using System;

namespace Reax.Lexer;

public static class Keywords
{
    public static byte[] LET => [(byte)'l', (byte)'e', (byte)'t'];

    public static byte[] IF => [(byte)'i', (byte)'f'];

    public static byte[] ELSE => [(byte)'e', (byte)'l', (byte)'s', (byte)'e'];

    public static byte[] ON => [(byte)'o', (byte)'n'];

    public static byte[] TRUE => [(byte)'t', (byte)'r', (byte)'u', (byte)'e'];

    public static byte[] FALSE => [(byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e'];

    public static byte[] FUN => [(byte)'f', (byte)'u', (byte)'n'];

    public static byte[] RETURN => [(byte)'r', (byte)'e', (byte)'t', (byte)'u', (byte)'r', (byte)'n'];

    public static byte[] WHEN => [(byte)'w', (byte)'h', (byte)'e', (byte)'n'];

    public static byte[] FOR => [(byte)'f', (byte)'o', (byte)'r'];
    
    public static byte[] TO => [(byte)'t', (byte)'o'];

    public static byte[] WHILE => [(byte)'w', (byte)'h', (byte)'i', (byte)'l', (byte)'e'];

    public static byte[] AND => [(byte)'a', (byte)'n', (byte)'d'];
    
    public static byte[] OR => [(byte)'o', (byte)'r'];
    
    public static byte[] NOT => [(byte)'n', (byte)'o', (byte)'t'];

    public static byte[] IMPORT => [(byte)'i', (byte)'m', (byte)'p', (byte)'o', (byte)'r', (byte)'t'];
    
    public static byte[] SCRIPT => [(byte)'s', (byte)'c', (byte)'r', (byte)'i', (byte)'p', (byte)'t'];
    
    public static byte[] MODULE => [(byte)'m', (byte)'o', (byte)'d', (byte)'u', (byte)'l', (byte)'e'];

    public static byte[] CONST => [(byte)'c', (byte)'o', (byte)'n', (byte)'s', (byte)'t'];

    public static TokenType IsKeyword(byte[] chars)
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
        if (MODULE.SequenceEqual(chars)) return TokenType.MODULE;
        if (CONST.SequenceEqual(chars)) return TokenType.CONST;
        return TokenType.IDENTIFIER;
    }
    
}
