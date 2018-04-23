using UnityEngine;
using System;

public static class NumberLiteralParser
{

    public static Number Parse(string input)
    {
        try
        {
            return new Number(Double.Parse(input));
        }
        catch (FormatException)
        {
            throw new SyntaxException("Number is in an invalid format");
        }
    }

}
