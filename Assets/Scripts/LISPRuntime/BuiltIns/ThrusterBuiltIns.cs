using UnityEngine;
using System.Collections.Generic;
using System;

public class ThrusterBuiltIns
{
    public static void DefineIn(Scope scope)
    {
        scope.AddVar(ThrusterState.LAUNCH_THRUSTER, ThrusterState.LAUNCH_THRUSTER);
        ThrusterState.SetPropellantState(ThrusterState.LAUNCH_THRUSTER, LispNull.NULL);
        ThrusterState.SetThrusterState(ThrusterState.LAUNCH_THRUSTER, LispNull.NULL);

        scope.AddCallable(new Symbol("SET-PROPELLANT-STATE"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>()
                    {
                        new Symbol("THRUSTER-ID"),
                        new Symbol("VALUE"),
                    },
                    restAccepted = false,
                },
                SetPropellantState,
                scope));

        scope.AddCallable(new Symbol("SPARK"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>()
                    {
                        new Symbol("THRUSTER-ID")
                    },
                    restAccepted = false,
                },
                Spark,
                scope));

        scope.AddCallable(new Symbol("GET-THRUSTER-TEMP"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>()
                    {
                        new Symbol("THRUSTER-ID")
                    },
                    restAccepted = false,
                },
                GetThrusterTemp,
                scope));
    }

    public static IEnumerable<LispExpression> SetPropellantState(Scope innerScope)
    {
        Symbol thrusterID = innerScope.Var(new Symbol("THRUSTER-ID")) as Symbol;
        if (object.ReferenceEquals(thrusterID, null))
        {
            throw new WrongArgTypeException(
                    "THRUSTER-ID",
                    typeof(Symbol),
                    innerScope.Var(new Symbol("THRUSTER-ID")).GetType());
        }
        LispExpression val = innerScope.Var(new Symbol("VALUE"));

        if (MainGeneratorState.Get() != LispBoolean.T)
        {
            throw new RuntimeException("Propellant cannot be powered with backup generator");
        }

        if (!ThrusterState.Has(thrusterID))
        {
            throw new RuntimeException("No thruster with ID " + thrusterID + " exists");
        }

        ThrusterState.SetPropellantState(thrusterID, val);

        yield return val;
    }

    public static IEnumerable<LispExpression> Spark(Scope innerScope)
    {
        Symbol thrusterID = innerScope.Var(new Symbol("THRUSTER-ID")) as Symbol;
        if (object.ReferenceEquals(thrusterID, null))
        {
            throw new WrongArgTypeException(
                    "THRUSTER-ID",
                    typeof(Symbol),
                    innerScope.Var(new Symbol("THRUSTER-ID")).GetType());
        }

        if (MainGeneratorState.Get() != LispBoolean.T)
        {
            throw new RuntimeException("Spark cannot be powered with backup generator");
        }

        if (!ThrusterState.Has(thrusterID))
        {
            throw new RuntimeException("No thruster with ID " + thrusterID + " exists");
        }

        if (ThrusterState.GetPropellantState(thrusterID) != LispNull.NULL)
        {
            if ((int) (UnityEngine.Random.value * 30) == 1)
            {
                ThrusterState.SetThrusterState(thrusterID, LispBoolean.T);
            }
        }

        yield return LispNull.NULL;
    }

    public static IEnumerable<LispExpression> GetThrusterTemp(Scope innerScope)
    {
        Symbol thrusterID = innerScope.Var(new Symbol("THRUSTER-ID")) as Symbol;
        if (object.ReferenceEquals(thrusterID, null))
        {
            throw new WrongArgTypeException(
                    "THRUSTER-ID",
                    typeof(Symbol),
                    innerScope.Var(new Symbol("THRUSTER-ID")).GetType());
        }

        if (!ThrusterState.Has(thrusterID))
        {
            throw new RuntimeException("No thruster with ID " + thrusterID + " exists");
        }

        if (ThrusterState.GetThrusterState(thrusterID) == LispBoolean.T)
        {
            yield return new Number(3316.56);
        }
        else
        {
            yield return new Number(-5.7);
        }
    }
}
