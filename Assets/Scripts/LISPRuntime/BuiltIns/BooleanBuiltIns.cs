using UnityEngine;
using System.Collections.Generic;
using System;

public class BooleanBuiltIns
{
    public static void DefineIn(Scope scope)
    {
        scope.AddVar(new Symbol("T"), LispBoolean.T);
        scope.AddVar(new Symbol("NIL"), LispNull.NULL);

        scope.AddCallable(new Symbol("NOT"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>(){new Symbol("x")},
                    restAccepted = false
                },
                Not,
                scope));
        scope.AddCallable(new Symbol("AND"), new SystemMacro(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>(){},
                    restAccepted = true
                },
                And,
                scope));
        scope.AddCallable(new Symbol("OR"), new SystemMacro(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>(){},
                    restAccepted = true
                },
                Or,
                scope));
    }

    public static IEnumerable<LispExpression> Not(Scope innerScope)
    {
        LispExpression x = innerScope.Var(new Symbol("x"));

        if (x != LispNull.NULL)
        {
            yield return LispNull.NULL;
        }
        else
        {
            yield return LispBoolean.T;
        }
    }

    public static IEnumerable<LispExpression> And(Scope innerScope)
    {
        LispList rest = innerScope.Var(new Symbol("rest")) as LispList;

        LispExpression last = LispBoolean.T;
        foreach (LispExpression arg in rest.ToCSharpList())
        {
            foreach (LispExpression exp in arg.Evaluate(innerScope))
            {
                last = exp;
                yield return null;
            }

            if (last == LispNull.NULL)
            {
                yield return LispNull.NULL;
                yield break; // Stop executing early
            }
        }

        yield return last;
    }

    public static IEnumerable<LispExpression> Or(Scope innerScope)
    {
        LispList rest = innerScope.Var(new Symbol("rest")) as LispList;

        foreach (LispExpression arg in rest.ToCSharpList())
        {
            LispExpression result = LispNull.NULL;
            foreach (LispExpression step in arg.Evaluate(innerScope))
            {
                result = step;
                yield return null;
            }

            if (result != LispNull.NULL)
            {
                yield return result;
                yield break; // Stop executing early
            }
        }

        yield return LispNull.NULL;
    }
}
