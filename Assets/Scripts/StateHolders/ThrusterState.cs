using UnityEngine;
using System.Collections.Generic;
using System;

public class ThrusterState
{
    public static Symbol LAUNCH_THRUSTER = new Symbol("LAUNCH-THRUSTER");

    private static Dictionary<Symbol, LispExpression> propellantState;
    private static Dictionary<Symbol, LispExpression> thrusterState;

    static ThrusterState()
    {
        propellantState = new Dictionary<Symbol, LispExpression>();
        thrusterState = new Dictionary<Symbol, LispExpression>();
    }

    public static bool Has(Symbol thrusterID)
    {
        return thrusterState.ContainsKey(thrusterID);
    }

    public static void SetPropellantState(Symbol thrusterID, LispExpression state)
    {
        Debug.Log("Propellant has been turned on for thruster " + thrusterID.ToString());
        propellantState[thrusterID] = state;
    }

    public static LispExpression GetPropellantState(Symbol thrusterID)
    {
        return propellantState[thrusterID];
    }

    public static void SetThrusterState(Symbol thrusterID, LispExpression state)
    {
        Debug.Log("Set thruster " + thrusterID.ToString() + " state to " + state.ToString());
        thrusterState[thrusterID] = state;
    }

    public static LispExpression GetThrusterState(Symbol thrusterID)
    {
        return thrusterState[thrusterID];
    }
}
