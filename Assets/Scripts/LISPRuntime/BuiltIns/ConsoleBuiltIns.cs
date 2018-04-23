using UnityEngine;
using System.Collections.Generic;
using System;

public class ConsoleBuiltIns
{
    public static void DefineIn(Scope scope)
    {
        scope.AddCallable(new Symbol("PRINT"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>(){new Symbol("object")},
                    restAccepted = true
                },
                Print,
                scope));
    }

    public static IEnumerable<LispExpression> Print(Scope innerScope)
    {
        LispExpression obj = innerScope.Var(new Symbol("object"));

        // TODO(velovix): Can this block?
        Debug.Log(obj.ToString());
        PrintState.Request(obj);

        yield return obj;
    }
}
