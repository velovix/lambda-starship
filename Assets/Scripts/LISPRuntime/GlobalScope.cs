using UnityEngine;
using System.Collections.Generic;
using System;

public class GlobalScope
{
    public static Scope Get()
    {
        Scope scope = new Scope(null);

        SpecialForms.DefineIn(scope);

        scope.AddCallable(new Symbol("DEFUN"), new SystemMacro(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>
                    {
                        new Symbol("FUNCTION-NAME"),
                        new Symbol("ARGUMENTS")
                    },
                    restAccepted = true
                },
                Defun,
                scope));
        scope.AddCallable(new Symbol("LAMBDA"), new SystemMacro(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("args")},
                    restAccepted = true
                },
                Lambda,
                scope));
        scope.AddCallable(new Symbol("DEFVAR"), new SystemMacro(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>
                    {
                        new Symbol("NAME"),
                        new Symbol("INITIAL-VALUE")
                    },
                    restAccepted = false
                },
                Defvar,
                scope));
        scope.AddCallable(new Symbol("DEFPARAMETER"), new SystemMacro(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>
                    {
                        new Symbol("NAME"),
                        new Symbol("INITIAL-VALUE")
                    },
                    restAccepted = false
                },
                Defparameter,
                scope));
        scope.AddCallable(new Symbol("DEFCONSTANT"), new SystemMacro(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>
                    {
                        new Symbol("NAME"),
                        new Symbol("INITIAL-VALUE")
                    },
                    restAccepted = false
                },
                Defconstant,
                scope));

        MathBuiltIns.DefineIn(scope);
        LightBuiltIns.DefineIn(scope);
        SwitchBuiltIns.DefineIn(scope);
        BooleanBuiltIns.DefineIn(scope);
        ConsoleBuiltIns.DefineIn(scope);
        ThrusterBuiltIns.DefineIn(scope);

        return scope;
    }

    public static IEnumerable<LispExpression> Defun(Scope innerScope)
    {
        Symbol functionName = innerScope.Var(new Symbol("FUNCTION-NAME")) as Symbol;
        if (object.ReferenceEquals(functionName, null))
        {
            throw new WrongArgTypeException(
                    "FUNCTION-NAME",
                    typeof(Symbol),
                    innerScope.Var(new Symbol("FUNCTION-NAME")).GetType());
        }
        LispList rawArguments = innerScope.Var(new Symbol("ARGUMENTS")) as LispList;
        if (object.ReferenceEquals(rawArguments, null))
        {
            throw new WrongArgTypeException(
                    "ARGUMENTS",
                    typeof(LispList),
                    innerScope.Var(new Symbol("ARGUMENTS")).GetType());
        }
        LispList body = innerScope.Var(new Symbol("rest")) as LispList;

        // Convert the arguments Lisp list to a list of symbols
        List<Symbol> arguments = new List<Symbol>();
        foreach (LispExpression rawArg in rawArguments.ToCSharpList())
        {
            arguments.Add(rawArg as Symbol);
        }
        
        // Construct the argument signature
        ArgumentSignature argSignature = new ArgumentSignature
        {
            namedArgs=arguments,
            restAccepted=false // TODO(velovix): Add varargs support for user functions
        };

        UserDefinedFunction func = new UserDefinedFunction(
                argSignature,
                body.ToCSharpList(),
                innerScope.Parent());

        // Traverse up to the global scope
        Scope globalScope = innerScope;
        while (globalScope.Parent() != null)
        {
            globalScope = innerScope.Parent();
        }
        // Define the function in the global scope
        globalScope.AddCallable(functionName, func);

        yield return functionName;
    }

    public static IEnumerable<LispExpression> Lambda(Scope innerScope)
    {
        LispList rawArguments = innerScope.Var(new Symbol("args")) as LispList;
        if (object.ReferenceEquals(rawArguments, null))
        {
            throw new WrongArgTypeException(
                    "ARGUMENTS",
                    typeof(LispList),
                    innerScope.Var(new Symbol("ARGUMENTS")).GetType());
        }
        LispList body = innerScope.Var(new Symbol("rest")) as LispList;

        // Convert the arguments Lisp list to a list of symbols
        List<Symbol> arguments = new List<Symbol>();
        foreach (LispExpression rawArg in rawArguments.ToCSharpList())
        {
            Symbol arg = rawArg as Symbol;
            if (object.ReferenceEquals(arg, null))
            {
                throw new RuntimeException("All arguments must be symbols");
            }
            arguments.Add(rawArg as Symbol);
        }

        // Construct the argument signature
        ArgumentSignature argSignature = new ArgumentSignature
        {
            namedArgs=arguments,
            restAccepted=false // TODO(velovix): Add varargs support for lambdas
        };

        UserDefinedFunction func = new UserDefinedFunction(
                argSignature, body.ToCSharpList(), innerScope.Parent());

        yield return new FunctionObject(func);
    }

    public static IEnumerable<LispExpression> Defvar(Scope innerScope)
    {
        Symbol name = innerScope.Var(new Symbol("NAME")) as Symbol;
        if (object.ReferenceEquals(name, null))
        {
            throw new WrongArgTypeException(
                    "NAME",
                    typeof(Symbol),
                    innerScope.Var(new Symbol("FUNCTION-NAME")).GetType());
        }
        LispExpression initialValue = innerScope.Var(new Symbol("INITIAL-VALUE"));

        // Traverse up to the global scope
        Scope globalScope = innerScope;
        while (globalScope.Parent() != null)
        {
            globalScope = innerScope.Parent();
        }

        // Make sure it isn't constant
        if (globalScope.IsConstant(name))
        {
            throw new RuntimeException("Cannot redefine constant value");
        }

        // Make sure it isn't already defined
        if (!globalScope.HasVar(name))
        {
            LispExpression result = null;
            foreach (LispExpression exp in initialValue.Evaluate(innerScope))
            {
                result = exp;
                yield return null;
            }
            globalScope.AddVar(name, result);
        }

        yield return name;
    }

    public static IEnumerable<LispExpression> Defparameter(Scope innerScope)
    {
        Symbol name = innerScope.Var(new Symbol("NAME")) as Symbol;
        if (object.ReferenceEquals(name, null))
        {
            throw new WrongArgTypeException(
                    "NAME",
                    typeof(Symbol),
                    innerScope.Var(new Symbol("FUNCTION-NAME")).GetType());
        }
        LispExpression initialValue = innerScope.Var(new Symbol("INITIAL-VALUE"));

        // Traverse up to the global scope
        Scope globalScope = innerScope;
        while (globalScope.Parent() != null)
        {
            globalScope = innerScope.Parent();
        }

        // Make sure it isn't constant
        if (globalScope.IsConstant(name))
        {
            throw new RuntimeException("Cannot redefine constant value");
        }

        LispExpression result = null;
        foreach (LispExpression exp in initialValue.Evaluate(innerScope))
        {
            result = exp;
            yield return null;
        }
        globalScope.AddVar(name, result);

        yield return name;
    }

    public static IEnumerable<LispExpression> Defconstant(Scope innerScope)
    {
        Symbol name = innerScope.Var(new Symbol("NAME")) as Symbol;
        if (object.ReferenceEquals(name, null))
        {
            throw new WrongArgTypeException(
                    "NAME",
                    typeof(Symbol),
                    innerScope.Var(new Symbol("FUNCTION-NAME")).GetType());
        }
        LispExpression initialValue = innerScope.Var(new Symbol("INITIAL-VALUE"));

        // Traverse up to the global scope
        Scope globalScope = innerScope;
        while (globalScope.Parent() != null)
        {
            globalScope = innerScope.Parent();
        }

        LispExpression result = null;
        foreach (LispExpression exp in initialValue.Evaluate(innerScope))
        {
            result = exp;
            yield return null;
        }
        globalScope.AddVar(name, result);
        globalScope.SetConstant(name, true);

        yield return name;
    }
}
