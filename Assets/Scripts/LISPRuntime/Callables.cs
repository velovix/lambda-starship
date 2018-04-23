using UnityEngine;
using System.Collections.Generic;
using System;

/**
 * Describes a Lisp expression that can be called with zero or more arguments.
 */
public interface LispCallable
{
    IEnumerable<LispExpression> Run(List<LispExpression> args, Scope callingScope);
}

/**
 * Describes any type of Lisp function.
 */
public interface LispFunction : LispCallable
{
    // As of right now, this interface only exists for taxonomy purposes
}

/**
 * Describes any type of Lisp macro.
 */
public interface LispMacro : LispCallable
{
    // As of right now, this interface only exists for taxonomy purposes
}

/**
 * Describes how arguments may be passed to a callable.
 */
public struct ArgumentSignature
{
    /**
     * A list of the names of all named arguments. All named arguments are
     * required and passed in order.
     */
    public List<Symbol> namedArgs;
    
    /**
     * True if the function accepts more arguments after all named arguments.
     */
    public bool restAccepted;
}

/**
 * A special form is a built-in macro-like callable that runs predefined C#
 * code. They cannot be created at runtime. Special forms can basically do
 * whatever they want.
 *
 * Special forms are macro-like because their arguments are not evaluated, but
 * are not actually macros.
 */
public class SpecialForm : LispCallable
{
    public delegate IEnumerable<LispExpression> Behavior(Scope innerScope);

    private ArgumentSignature argSignature;
    private Behavior behavior;
    private Scope closure;

    public SpecialForm(ArgumentSignature argSignature, Behavior behavior, Scope scope)
    {
        this.argSignature = argSignature;
        this.behavior = behavior;
        this.closure = scope;
    }

    public IEnumerable<LispExpression> Run(List<LispExpression> args, Scope callingScope)
    {
        Scope innerScope = CallableUtils.MakeInnerScope(
                closure, callingScope, argSignature, args, true);

        foreach (LispExpression exp in behavior(innerScope))
        {
            yield return exp;
        }
    }
}

/**
 * A built-in function that calls predefined C# code. SystemFunctions can be
 * used like user-defined functions, they just don't have a Lisp body.
 */
public class SystemFunction : LispCallable, LispFunction
{
    public delegate IEnumerable<LispExpression> Behavior(Scope innerScope);

    private ArgumentSignature argSignature;
    private Behavior behavior;
    private Scope closure;

    public SystemFunction(ArgumentSignature argSignature, Behavior behavior, Scope scope)
    {
        this.argSignature = argSignature;
        this.behavior = behavior;
        this.closure = scope;
    }

    public IEnumerable<LispExpression> Run(List<LispExpression> args, Scope callingScope)
    {
        Scope innerScope = CallableUtils.MakeInnerScope(
                closure, callingScope, argSignature, args, false);

        foreach (LispExpression exp in behavior(innerScope))
        {
            yield return exp;
        }
    }
}

public class SystemMacro : LispCallable, LispMacro
{
    public delegate IEnumerable<LispExpression> Behavior(Scope innerScope);

    private ArgumentSignature argSignature;
    private Behavior behavior;
    private Scope closure;

    public SystemMacro(ArgumentSignature argSignature, Behavior behavior, Scope scope)
    {
        this.argSignature = argSignature;
        this.behavior = behavior;
        this.closure = scope;
    }

    public IEnumerable<LispExpression> Run(List<LispExpression> args, Scope callingScope)
    {
        Scope innerScope = CallableUtils.MakeInnerScope(
                closure, callingScope, argSignature, args, true);

        foreach (LispExpression exp in behavior(innerScope))
        {
            yield return exp;
        }
    }
}

/**
 * A function defined at runtime using DEFUN or LAMBDA. Contains a body of Lisp
 * code to be run.
 */
public class UserDefinedFunction : LispCallable, LispFunction
{
    private ArgumentSignature argSignature;
    private List<LispExpression> body;
    private Scope closure;

    public UserDefinedFunction(ArgumentSignature argSignature, List<LispExpression> body, Scope scope)
    {
        this.argSignature = argSignature;
        this.body = body;
        this.closure = scope;
    }

    public IEnumerable<LispExpression> Run(List<LispExpression> args, Scope callingScope)
    {
        Scope innerScope = CallableUtils.MakeInnerScope(
                closure, callingScope, argSignature, args, false);

        LispExpression output = LispNull.NULL;
        foreach (LispExpression bodyPart in body)
        {
            foreach (LispExpression exp in bodyPart.Evaluate(innerScope))
            {
                output = exp;
                yield return null;
            }
        }

        // Return the last evaluated expression
        yield return output;
    }
}

public class CallableUtils
{
    public static Scope MakeInnerScope(
            Scope closure,
            Scope callingScope,
            ArgumentSignature argSignature,
            List<LispExpression> args,
            bool isMacro)
    {
        Scope innerScope = new Scope(closure);

        if (isMacro)
        {
            // Introduce the calling scope. This isn't the right way to do this
            // TODO(velovix): Find a better way to implement lazy evaluation
            innerScope.Combine(callingScope);
        }

        if (argSignature.restAccepted)
        {
            if (args.Count < argSignature.namedArgs.Count)
            {
                throw new RuntimeException("Invalid number of args");
            }

            // Put the rest of the arguments in the "rest" variable
            List<LispExpression> rest = new List<LispExpression>();
            if (args.Count > argSignature.namedArgs.Count)
            {
                foreach (LispExpression expr in args.GetRange(argSignature.namedArgs.Count, args.Count - argSignature.namedArgs.Count))
                {
                    if (isMacro)
                    {
                        rest.Add(expr);
                    }
                    else
                    {
                        LispExpression result = null;
                        foreach (LispExpression exp in expr.Evaluate(callingScope))
                        {
                            result = exp;
                        }
                        rest.Add(result);
                    }
                }
            }
            innerScope.AddVar(new Symbol("rest"), new LispList(rest));
        }
        else
        {
            if (args.Count != argSignature.namedArgs.Count)
            {
                throw new RuntimeException("Invalid number of args");
            }
        }

        for (int i=0; i<argSignature.namedArgs.Count; i++)
        {
            if (isMacro)
            {
                innerScope.AddVar(argSignature.namedArgs[i], args[i]);
            }
            else
            {
                LispExpression result = null;
                foreach (LispExpression exp in args[i].Evaluate(callingScope))
                {
                    result = exp;
                }
                innerScope.AddVar(argSignature.namedArgs[i], result);
            }
        }

        return innerScope;
    }
}
