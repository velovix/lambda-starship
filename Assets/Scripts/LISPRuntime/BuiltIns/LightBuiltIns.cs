using UnityEngine;
using System.Collections.Generic;
using System;

public class LightBuiltIns
{
    public static void DefineIn(Scope scope)
    {
        scope.AddCallable(new Symbol("ACTIVATE-GENERATOR"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>(){},
                    restAccepted = false
                },
                ActivateGenerator,
                scope));

        Symbol mainLight = new Symbol("MAIN-LIGHT");
        scope.AddVar(mainLight, mainLight);
        LightState.Set(mainLight, LispNull.NULL);

        scope.AddCallable(new Symbol("SET-LIGHT-STATE"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>()
                    {
                        new Symbol("LIGHT-ID"),
                        new Symbol("VALUE")
                    },
                    restAccepted = false,
                },
                SetLightPower,
                scope));
    }

    public static IEnumerable<LispExpression> ActivateGenerator(Scope innerScope)
    {
        MainGeneratorState.Set(LispBoolean.T);

        yield return LispBoolean.T;
    }

    public static IEnumerable<LispExpression> SetLightPower(Scope innerScope)
    {
        Symbol lightID = innerScope.Var(new Symbol("LIGHT-ID")) as Symbol;
        if (object.ReferenceEquals(lightID, null))
        {
            throw new WrongArgTypeException(
                    "LIGHT-ID",
                    typeof(Symbol),
                    innerScope.Var(new Symbol("LIGHT-ID")).GetType());
        }
        LispExpression val = innerScope.Var(new Symbol("VALUE"));

        if (MainGeneratorState.Get() != LispBoolean.T)
        {
            throw new RuntimeException("Light cannot be powered with backup generator");
        }

        LightState.Set(lightID, val);

        yield return val;
    }
}
