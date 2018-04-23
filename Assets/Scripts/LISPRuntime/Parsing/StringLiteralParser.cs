using System.Collections.Generic;
using System;
using UnityEngine;

public class StringLiteralParser
{
    public enum State
    {
        Start,
        Consuming,
        Escaping,
        Terminated
    }

    private static Dictionary<char, string> escapeSequences = new Dictionary<char, string>
    {
        { 'n', "\n" },
        { 't', "\t" },
        { '\\', "\\" },
        { '\"', "\"" }
    };

    public static StringLiteral Parse(string input)
    {
        State state = State.Start;

        string output = "";

        for (int i=0; i<input.Length; i++)
        {
            char c = input[i];

            if (state == State.Start)
            {
                if (c != '\"')
                {
                    throw new SyntaxException("String literal does not start with a double quote");
                }
                state = State.Consuming;
            }
            else if (state == State.Consuming)
            {
                if (c == '\\')
                {
                    state = State.Escaping;
                }
                else if (c == '\"')
                {
                    if (i == input.Length-1)
                    {
                        // The string terminated normally
                        state = State.Terminated;
                    }
                    else
                    {
                        // The string is terminated before the input ends
                        throw new SyntaxException("Trailing characters after string termination");
                    }
                }
                else 
                {
                    output += c;
                }
            }
            else if (state == State.Escaping)
            {
                if (escapeSequences.ContainsKey(c))
                {
                    output += escapeSequences[c];
                    state = State.Consuming;
                }
                else
                {
                    throw new SyntaxException("Unknown escape sequence");
                }
            }
        }

        if (state != State.Terminated)
        {
            throw new SyntaxException("Unterminated string literal");
        }

        return new StringLiteral(output);
    }
}
