using UnityEngine;
using System.Collections.Generic;
using System;

public struct PrintEvent
{
    public LispExpression printable;

    public PrintEvent(LispExpression printable)
    {
        this.printable = printable;
    }
}

public class PrintState
{
    private static Queue<PrintEvent> events;

    static PrintState()
    {
        events = new Queue<PrintEvent>();
    }

    public static void Request(LispExpression printable)
    {
        events.Enqueue(new PrintEvent(printable));
    }

    public static List<PrintEvent> GetPrintEvents()
    {
        List<PrintEvent> output = new List<PrintEvent>();

        while (events.Count > 0)
        {
            output.Add(events.Dequeue());
        }

        return output;
    }
}
