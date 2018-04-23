using UnityEngine;
using System.Collections.Generic;
using System;

public class MathBuiltIns
{
    public static void DefineIn(Scope scope)
    {
        scope.AddCallable(new Symbol("+"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{},
                    restAccepted = true
                },
                Add,
                scope));
        scope.AddCallable(new Symbol("-"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{},
                    restAccepted = true
                },
                Subtract,
                scope));
        scope.AddCallable(new Symbol("*"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{},
                    restAccepted = true
                },
                Multiply,
                scope));
        scope.AddCallable(new Symbol("/"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("n1")},
                    restAccepted = true
                },
                Divide,
                scope));
        scope.AddCallable(new Symbol(">"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("n1")},
                    restAccepted = true,
                },
                GreaterThan,
                scope));
        scope.AddCallable(new Symbol("<"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("n1")},
                    restAccepted = true,
                },
                LessThan,
                scope));
        scope.AddCallable(new Symbol("ABS"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("n")},
                    restAccepted = false
                },
                Abs,
                scope));
        scope.AddCallable(new Symbol("SIGNUM"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("n")},
                    restAccepted = false
                },
                Signum,
                scope));
        scope.AddCallable(new Symbol("SIN"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("n")},
                    restAccepted = false
                },
                Sin,
                scope));
        scope.AddCallable(new Symbol("COS"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("n")},
                    restAccepted = false
                },
                Cos,
                scope));
        scope.AddCallable(new Symbol("TAN"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("n")},
                    restAccepted = false
                },
                Tan,
                scope));
        scope.AddCallable(new Symbol("SINH"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("n")},
                    restAccepted = false
                },
                Sinh,
                scope));
        scope.AddCallable(new Symbol("COSH"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("n")},
                    restAccepted = false
                },
                Cosh,
                scope));
        scope.AddCallable(new Symbol("TANH"), new SystemFunction(
                new ArgumentSignature()
                {
                    namedArgs = new List<Symbol>{new Symbol("n")},
                    restAccepted = false
                },
                Tanh,
                scope));

        scope.AddVar(new Symbol("PI"), new Number(Math.PI));
    }

    public static IEnumerable<LispExpression> Add(Scope innerScope)
    {
        double result = 0;
        LispList args = innerScope.Var(new Symbol("rest")) as LispList;
        for (int i=0; i<args.Count(); i++)
        {
            Number num = args[i] as Number;
            if (object.ReferenceEquals(num, null))
            {
                throw new RuntimeException("All arguments to + must be numbers");
            }
            result += num.val;
        }

        yield return new Number(result);
    }

    public static IEnumerable<LispExpression> Subtract(Scope innerScope)
    {
        double result = 0;
        LispList args = innerScope.Var(new Symbol("rest")) as LispList;
        for (int i=0; i<args.Count(); i++)
        {
            Number num = args[i] as Number;
            if (object.ReferenceEquals(num, null))
            {
                throw new RuntimeException("All arguments to - must be numbers");
            }
            result -= num.val;
        }

        yield return new Number(result);
    }

    public static IEnumerable<LispExpression> Multiply(Scope innerScope)
    {
        double result = 1;
        LispList args = innerScope.Var(new Symbol("rest")) as LispList;
        if (args.Count() == 0)
        {
            // The identity of multiplication is zero
            result = 0.0;
        }
        else
        {
            for (int i=0; i<args.Count(); i++)
            {
                Number num = args[i] as Number;
                if (object.ReferenceEquals(num, null))
                {
                    throw new RuntimeException("All arguments to * must be numbers");
                }
                result *= num.val;
            }
        }

        yield return new Number(result);
    }

    public static IEnumerable<LispExpression> Divide(Scope innerScope)
    {
        double result = 0;
        Number firstArg = innerScope.Var(new Symbol("n1")) as Number;
        if (object.ReferenceEquals(firstArg, null))
        {
            throw new RuntimeException("All arguments to / must be numbers");
        }
        LispList rest = innerScope.Var(new Symbol("rest")) as LispList;

        if (rest.Count() == 0)
        {
            result = 1.0 / firstArg.val;
        }
        else
        {
            result = firstArg.val;
            for (int i=0; i<rest.Count(); i++)
            {
                Number num = rest[i] as Number;
                if (object.ReferenceEquals(num, null))
                {
                    throw new RuntimeException("All arguments to / must be numbers");
                }
                result /= num.val;
            }
        }

        yield return new Number(result);
    }

    public static IEnumerable<LispExpression> GreaterThan(Scope innerScope)
    {
        Number firstArg = innerScope.Var(new Symbol("n1")) as Number;
        if (object.ReferenceEquals(firstArg, null))
        {
            throw new RuntimeException("All arguments to > must be numbers");
        }
        LispList rest = innerScope.Var(new Symbol("rest")) as LispList;

        Number currNumber = firstArg;
        for (int i=0; i<rest.Count(); i++)
        {
            Number num = rest[i] as Number;
            if (object.ReferenceEquals(num, null))
            {
                throw new RuntimeException("All arguments to > must be numbers");
            }
            if (currNumber.val <= num.val)
            {
                yield return LispNull.NULL;
                yield break; // Stop execution early
            }
            currNumber = num;
        }

        yield return LispBoolean.T;
    }

    public static IEnumerable<LispExpression> LessThan(Scope innerScope)
    {
        Number firstArg = innerScope.Var(new Symbol("n1")) as Number;
        if (object.ReferenceEquals(firstArg, null))
        {
            throw new RuntimeException("All arguments to < must be numbers");
        }
        LispList rest = innerScope.Var(new Symbol("rest")) as LispList;

        Number currNumber = firstArg;
        for (int i=0; i<rest.Count(); i++)
        {
            Number num = rest[i] as Number;
            if (object.ReferenceEquals(num, null))
            {
                throw new RuntimeException("All arguments to < must be numbers");
            }
            if (currNumber.val >= num.val)
            {
                yield return LispNull.NULL;
                yield break; // Stop execution early
            }
            currNumber = num;
        }

        yield return LispBoolean.T;
    }

    public static IEnumerable<LispExpression> Abs(Scope innerScope)
    {
        Number n = innerScope.Var(new Symbol("n")) as Number;
        if (object.ReferenceEquals(n, null))
        {
            throw new WrongArgTypeException(
                    "n",
                    typeof(Number),
                    innerScope.Var(new Symbol("n")).GetType());
        }
        yield return new Number(Math.Abs(n.val));
    }

    public static IEnumerable<LispExpression> Signum(Scope innerScope)
    {
        Number n = innerScope.Var(new Symbol("n")) as Number;
        if (object.ReferenceEquals(n, null))
        {
            throw new WrongArgTypeException(
                    "n",
                    typeof(Number),
                    innerScope.Var(new Symbol("n")).GetType());
        }
        yield return new Number(Math.Sign(n.val));
    }

    public static IEnumerable<LispExpression> Sin(Scope innerScope)
    {
        Number n = innerScope.Var(new Symbol("n")) as Number;
        if (object.ReferenceEquals(n, null))
        {
            throw new WrongArgTypeException(
                    "n",
                    typeof(Number),
                    innerScope.Var(new Symbol("n")).GetType());
        }
        yield return new Number(Math.Sin(n.val));
    }

    public static IEnumerable<LispExpression> Cos(Scope innerScope)
    {
        Number n = innerScope.Var(new Symbol("n")) as Number;
        if (object.ReferenceEquals(n, null))
        {
            throw new WrongArgTypeException(
                    "n",
                    typeof(Number),
                    innerScope.Var(new Symbol("n")).GetType());
        }
        yield return new Number(Math.Cos(n.val));
    }

    public static IEnumerable<LispExpression> Tan(Scope innerScope)
    {
        Number n = innerScope.Var(new Symbol("n")) as Number;
        if (object.ReferenceEquals(n, null))
        {
            throw new WrongArgTypeException(
                    "n",
                    typeof(Number),
                    innerScope.Var(new Symbol("n")).GetType());
        }
        yield return new Number(Math.Tan(n.val));
    }

    public static IEnumerable<LispExpression> Asin(Scope innerScope)
    {
        Number n = innerScope.Var(new Symbol("n")) as Number;
        if (object.ReferenceEquals(n, null))
        {
            throw new WrongArgTypeException(
                    "n",
                    typeof(Number),
                    innerScope.Var(new Symbol("n")).GetType());
        }
        yield return new Number(Math.Asin(n.val));
    }

    public static IEnumerable<LispExpression> Acos(Scope innerScope)
    {
        Number n = innerScope.Var(new Symbol("n")) as Number;
        if (object.ReferenceEquals(n, null))
        {
            throw new WrongArgTypeException(
                    "n",
                    typeof(Number),
                    innerScope.Var(new Symbol("n")).GetType());
        }
        yield return new Number(Math.Acos(n.val));
    }

    public static IEnumerable<LispExpression> Sinh(Scope innerScope)
    {
        Number n = innerScope.Var(new Symbol("n")) as Number;
        if (object.ReferenceEquals(n, null))
        {
            throw new WrongArgTypeException(
                    "n",
                    typeof(Number),
                    innerScope.Var(new Symbol("n")).GetType());
        }
        yield return new Number(Math.Sinh(n.val));
    }

    public static IEnumerable<LispExpression> Cosh(Scope innerScope)
    {
        Number n = innerScope.Var(new Symbol("n")) as Number;
        if (object.ReferenceEquals(n, null))
        {
            throw new WrongArgTypeException(
                    "n",
                    typeof(Number),
                    innerScope.Var(new Symbol("n")).GetType());
        }
        yield return new Number(Math.Cosh(n.val));
    }

    public static IEnumerable<LispExpression> Tanh(Scope innerScope)
    {
        Number n = innerScope.Var(new Symbol("n")) as Number;
        if (object.ReferenceEquals(n, null))
        {
            throw new WrongArgTypeException(
                    "n",
                    typeof(Number),
                    innerScope.Var(new Symbol("n")).GetType());
        }
        yield return new Number(Math.Tanh(n.val));
    }
}
