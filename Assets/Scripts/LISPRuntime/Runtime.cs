using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class Runtime
{
    private static Scope globalScope;

    private static bool stopping = false;

    public static void Initialize()
    {
        globalScope = GlobalScope.Get();
    }

    public static IEnumerable<LispExpression> Run(LispExpression exp)
    {
        LispExpression output = LispNull.NULL;
        foreach (LispExpression step in exp.Evaluate(globalScope))
        {
            output = step;
            yield return null;
        }

        yield return output;
    }

    public static void Stop()
    {
        stopping = true;
    }

    public static bool IsStopping()
    {
        return stopping;
    }

    public static IEnumerable<LispExpression> CheckSwitchEvents()
    {
        List<SwitchEvent> events = SwitchState.GetSwitchEvents();
        foreach (SwitchEvent e in events)
        {
            if (SwitchState.HasCallback(e.switchID))
            {
                FunctionObject callback = SwitchState.GetCallback(e.switchID);

                List<LispExpression> callbackArgs = new List<LispExpression>()
                {
                    e.switchID,
                    e.val,
                };

                // Call the callback
                foreach (LispExpression exp in callback.val.Run(callbackArgs, globalScope))
                {
                    yield return exp;
                }
            }
        }
    }
}
