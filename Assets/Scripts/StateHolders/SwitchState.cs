using UnityEngine;
using System.Collections.Generic;
using System;

public struct SwitchEvent
{
    public Symbol switchID;
    public LispExpression val;

    public SwitchEvent(Symbol switchID, LispExpression val)
    {
        this.switchID = switchID;
        this.val = val;
    }
}

public class SwitchState
{
    private static Dictionary<Symbol, LispExpression> states;
    private static Dictionary<Symbol, FunctionObject> callbacks;

    private static Queue<SwitchEvent> switchEvents;

    static SwitchState()
    {
        states = new Dictionary<Symbol, LispExpression>();
        callbacks = new Dictionary<Symbol, FunctionObject>();
        switchEvents = new Queue<SwitchEvent>();
    }

    public static LispExpression Get(Symbol switchID)
    {
        return states[switchID];
    }

    public static bool Has(Symbol switchID)
    {
        return states.ContainsKey(switchID);
    }

    public static void Add(Symbol switchID, LispExpression initial)
    {
        states[switchID] = initial;
    }

    public static void Toggle(Symbol switchID)
    {
        if (states[switchID] == LispNull.NULL)
        {
            Set(switchID, LispBoolean.T);
        }
        else
        {
            Set(switchID, LispNull.NULL);
        }
    }

    public static List<SwitchEvent> GetSwitchEvents()
    {
        List<SwitchEvent> events = new List<SwitchEvent>();

        while (switchEvents.Count > 0)
        {
            events.Add(switchEvents.Dequeue());
        }

        return events;
    }

    public static void Set(Symbol switchID, LispExpression val)
    {
        if (!states.ContainsKey(switchID) || states[switchID] != val)
        {
            states[switchID] = val;

            switchEvents.Enqueue(new SwitchEvent(switchID, val));
        }
    }

    public static void SetCallback(Symbol switchID, FunctionObject function)
    {
        callbacks[switchID] = function;
    }

    public static bool HasCallback(Symbol switchID)
    {
        return callbacks.ContainsKey(switchID);
    }

    public static FunctionObject GetCallback(Symbol switchID)
    {
        return callbacks[switchID];
    }

    public static void ClearCallbacks()
    {
        callbacks = new Dictionary<Symbol, FunctionObject>();
    }
}
