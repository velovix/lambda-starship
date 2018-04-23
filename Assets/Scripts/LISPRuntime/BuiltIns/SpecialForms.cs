using UnityEngine;
using System.Collections.Generic;
using System;

public class SpecialForms
{

    public static void DefineIn(Scope scope)
    {
        scope.AddCallable(new Symbol("QUOTE"), new SpecialForm(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("obj")},
                    restAccepted = false
                },
                Quote,
                scope));

        scope.AddCallable(new Symbol("FUNCTION"), new SpecialForm(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("INPUT")},
                    restAccepted = true
                },
                Function,
                scope));
        scope.AddCallable(new Symbol("IF"), new SpecialForm(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>
                    {
                        new Symbol("condition"),
                        new Symbol("then")
                    },
                    restAccepted = true
                },
                If,
                scope));
        scope.AddCallable(new Symbol("EVAL"), new SpecialForm(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>
                    {
                        new Symbol("thing"),
                    },
                    restAccepted = false,
                },
                Eval,
                scope));
    }

    public static IEnumerable<LispExpression> Quote(Scope innerScope)
    {
        LispExpression obj = innerScope.Var(new Symbol("obj"));
        yield return obj;
    }

    public static IEnumerable<LispExpression> Function(Scope innerScope)
    {
        Symbol input = innerScope.Var(new Symbol("INPUT")) as Symbol;
        if (object.ReferenceEquals(input, null))
        {
            throw new WrongArgTypeException(
                    "INPUT",
                    typeof(Symbol),
                    innerScope.Var(new Symbol("INPUT")).GetType());
        }

        LispFunction func = innerScope.Parent().Callable(input) as LispFunction;
        if (object.ReferenceEquals(func, null))
        {
            throw new RuntimeException("Symbol does not reference a callable");
        }

        // TODO(velovix): In clisp, this special form has many odd uses that
        // may need to be implemented

        yield return new FunctionObject(func);
    }

    public static IEnumerable<LispExpression> If(Scope innerScope)
    {
        LispExpression condition = innerScope.Var(new Symbol("condition"));
        LispExpression thenResult = innerScope.Var(new Symbol("then"));
        LispList rest = innerScope.Var(new Symbol("rest")) as LispList;

        if (rest.Count() > 1)
        {
            throw new RuntimeException("Too many arguments");
        }

        LispExpression conditionResult = null;
        foreach (LispExpression exp in condition.Evaluate(innerScope))
        {
            conditionResult = exp;
            yield return null;
        }

        if (conditionResult != LispNull.NULL)
        {
            LispExpression result = null;
            foreach (LispExpression exp in thenResult.Evaluate(innerScope))
            {
                result = exp;
                yield return null;
            }
            yield return result;
            yield break; // Stop early
        }
        else if (rest.Count() == 1)
        {
            LispExpression result = null;
            foreach (LispExpression exp in rest[0].Evaluate(innerScope))
            {
                result = exp;
                yield return null;
            }
            yield return result;
            yield break; // Stop early
        }
        else
        {
            yield return LispNull.NULL;
        }
    }

    public static IEnumerable<LispExpression> Eval(Scope innerScope)
    {
        LispExpression thing = innerScope.Var(new Symbol("thing"));

        LispExpression result = null;
        foreach (LispExpression exp in thing.Evaluate(innerScope))
        {
            result = exp;
            yield return null;
        }
        yield return result;
    }

}
