using UnityEngine;
using System.Collections.Generic;
using System;

public class MainGeneratorState
{
    private static LispExpression state;

    public static void Set(LispExpression st)
    {
        Debug.Log("Main generator has been turned on!");
        state = st;
    }

    public static LispExpression Get()
    {
        return state;
    }
}
