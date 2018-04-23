using UnityEngine;
using System.Collections.Generic;
using System;

public class WarpDriveLeverState
{
    private static LispExpression state;

    static WarpDriveLeverState()
    {
        state = LispNull.NULL;
    }

    public static LispExpression Get()
    {
        return state;
    }

    public static void Set(LispExpression state)
    {
        WarpDriveLeverState.state = state;
    }

    public static void Toggle()
    {
        if (state == LispNull.NULL)
        {
            Set(LispBoolean.T);
        }
        else
        {
            Set(LispNull.NULL);
        }
    }
}
