using UnityEngine;
using System.Collections.Generic;
using System;

public class SwitchBuiltIns
{
    public static void DefineIn(Scope scope)
    {
        Symbol rightWallSwitch = new Symbol("RIGHT-WALL-SWITCH");
        scope.AddVar(rightWallSwitch, rightWallSwitch);
        SwitchState.Set(rightWallSwitch, LispNull.NULL);

        scope.AddCallable(new Symbol("ON-SWITCH-CHANGE"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>()
                    {
                        new Symbol("SWITCH-ID"),
                        new Symbol("CALLBACK")
                    },
                    restAccepted = false
                },
                OnSwitchChange,
                scope));
        scope.AddCallable(new Symbol("GET-SWITCH-STATE"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>(){new Symbol("SWITCH-ID")},
                    restAccepted = false
                },
                GetSwitchState,
                scope));
    }

    public static IEnumerable<LispExpression> OnSwitchChange(Scope innerScope)
    {
        Symbol switchID = innerScope.Var(new Symbol("SWITCH-ID")) as Symbol;
        if (object.ReferenceEquals(switchID, null))
        {
            throw new WrongArgTypeException(
                    "SWITCH-ID",
                    typeof(Symbol),
                    innerScope.Var(new Symbol("SWITCH-ID")).GetType());
        }
        FunctionObject callback = innerScope.Var(new Symbol("CALLBACK")) as FunctionObject;
        if (object.ReferenceEquals(callback, null))
        {
            throw new WrongArgTypeException(
                    "CALLBACK",
                    typeof(FunctionObject),
                    innerScope.Var(new Symbol("CALLBACK")).GetType());
        }

        SwitchState.SetCallback(switchID, callback);

        yield return callback;
    }

    public static IEnumerable<LispExpression> GetSwitchState(Scope innerScope)
    {
        Symbol switchID = innerScope.Var(new Symbol("SWITCH-ID")) as Symbol;
        if (object.ReferenceEquals(switchID, null))
        {
            throw new WrongArgTypeException(
                    "SWITCH-ID",
                    typeof(Symbol),
                    innerScope.Var(new Symbol("SWITCH-ID")).GetType());
        }

        if (!SwitchState.Has(switchID))
        {
            throw new RuntimeException("No such switch with ID " + switchID + " exists");
        }

        yield return SwitchState.Get(switchID);
    }
}
