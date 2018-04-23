using UnityEngine;
using System.Collections.Generic;
using System;

public class Scope
{
    private Scope parent;
    private Dictionary<Symbol, LispCallable> callables;
    private Dictionary<Symbol, LispExpression> variables;
    private Dictionary<Symbol, bool> isConstant;

    public Scope(Scope parent)
    {
        this.parent = parent;
        callables = new Dictionary<Symbol, LispCallable>();
        variables = new Dictionary<Symbol, LispExpression>();
        isConstant = new Dictionary<Symbol, bool>();
    }

    public void AddCallable(Symbol name, LispCallable callable)
    {
        callables[name] = callable;
    }

    public void AddVar(Symbol name, LispExpression val)
    {
        variables[name] = val;
    }

    public void SetConstant(Symbol name, bool constant)
    {
        isConstant[name] = constant;
    }

    public bool IsConstant(Symbol name)
    {
        if (!isConstant.ContainsKey(name))
        {
            return false;
        }
        return isConstant[name];
    }

    public LispCallable Callable(Symbol symbol)
    {
        if (callables.ContainsKey(symbol))
        {
            return callables[symbol];
        }

        if (this.parent != null)
        {
            return parent.Callable(symbol);
        }

        return null;
    }

    public LispExpression Var(Symbol symbol)
    {
        if (variables.ContainsKey(symbol))
        {
            return variables[symbol];
        }

        if (this.parent != null)
        {
            return parent.Var(symbol);
        }

        throw new RuntimeException("Variable " + symbol + " has no value");
    }

    public bool HasVar(Symbol symbol)
    {
        return variables.ContainsKey(symbol);
    }

    public Scope Parent()
    {
        return parent;
    }

    public void Combine(Scope scope)
    {
        foreach (KeyValuePair<Symbol, LispExpression> var in scope.variables)
        {
            variables[var.Key] = var.Value;
        }
    }

    public Scope Copy()
    {
        Scope copy = new Scope(parent);

        foreach (KeyValuePair<Symbol, LispCallable> entry in callables)
        {
            copy.AddCallable(entry.Key, entry.Value);
        }
        foreach (KeyValuePair<Symbol, LispExpression> entry in variables)
        {
            copy.AddVar(entry.Key, entry.Value);
        }

        return copy;
    }

    public override string ToString()
    {
        string output = "Callables:\n";
        foreach (KeyValuePair<Symbol, LispCallable> entry in callables)
        {
            output += "    \"" + entry.Key + "\": " + entry.Value.ToString() + "\n";
        }
        output += "Variables:\n";
        foreach (KeyValuePair<Symbol, LispExpression> entry in variables)
        {
            output += "    \"" + entry.Key + "\": " + entry.Value.ToString() + "\n";
        }

        return output;
    }
}
