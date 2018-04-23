using System;

public class SymbolParser
{

    // A string of all valid characters in symbols
    const string validSymbolChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ!$%&*+-./:<=>?@^_~";

    public static Symbol Parse(string input)
    {
        string output = "";

        // Lisp symbols are case insensitive
        input = input.ToUpper();

        for (int i=0; i<input.Length; i++)
        {
            char c = input[i];

            if (validSymbolChars.Contains(c.ToString()))
            {
                output += input[i];
            }
            else
            {
                throw new SyntaxException("Invalid character in symbol \"" + c + "\"");
            }
        }

        return new Symbol(output);
    }

}
