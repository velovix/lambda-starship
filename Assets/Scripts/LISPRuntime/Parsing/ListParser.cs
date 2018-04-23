using System.Text.RegularExpressions;
using System;
using UnityEngine;

public static class ListParser
{
    public static LispList Parse(Lexer lexer)
    {
        Regex numberPattern = new Regex("^((\\+|\\-)[0-9].*|[0-9].*)$");
        LispList output = new LispList();

        while (lexer.HasNext())
        {
            Lexeme lexeme = lexer.Next();

            if (lexeme.val == "(")
            {
                output.Add(Parse(lexer));
            }
            else if (lexeme.val == ")")
            {
                return output;
            }
            else if (numberPattern.Match(lexeme.val).Success)
            {
                output.Add(NumberLiteralParser.Parse(lexeme.val));
            }
            else if (lexeme.val[0] == '\"')
            {
                output.Add(StringLiteralParser.Parse(lexeme.val));
            }
            else
            {
                output.Add(SymbolParser.Parse(lexeme.val));
            }
        }

        throw new SyntaxException("Unterminated list");
    }
}
