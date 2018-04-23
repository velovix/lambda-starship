using UnityEngine;
using System.Collections.Generic;
using System;

public class LightState
{
    private static Dictionary<Symbol, LispExpression> states;

    static LightState()
    {
        states = new Dictionary<Symbol, LispExpression>();
    }

    public static void Add(Symbol lightID, LispExpression state)
    {
        states[lightID] = state;
    }

    public static void Set(Symbol lightID, LispExpression state)
    {
        Debug.Log("State switched in light " + lightID);
        states[lightID] = state;
    }

    public static LispExpression Get(Symbol lightID)
    {
        return states[lightID];
    }
}
