using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class LispExpression
{
    abstract public IEnumerable<LispExpression> OnEvaluation(Scope scope);

    public IEnumerable<LispExpression> Evaluate(Scope scope)
    {
        if (Runtime.IsStopping())
        {
            throw new EarlyStopException();
        }

        return OnEvaluation(scope);
    }
}

public class LispList : LispExpression
{

    private List<LispExpression> val;

    public LispList()
    {
        this.val = new List<LispExpression>();
    }

    public LispList(List<LispExpression> val)
    {
        this.val = val;
    }

    public LispExpression Head()
    {
        return val[0];
    }

    public LispList Tail()
    {
        return new LispList(val.GetRange(1, val.Count-1));
    }

    public LispExpression this[int index]
    {
        get { return val[index]; }
        set { val[index] = value; }
    }

    public int Count()
    {
        return val.Count;
    }

    public void Add(LispExpression exp)
    {
        val.Add(exp);
    }

    public override IEnumerable<LispExpression> OnEvaluation(Scope scope)
    {
        if (val.Count == 0)
        {
            yield return LispNull.NULL;
        }
        if (!(Head() is Symbol))
        {
            throw new RuntimeException("Callable name must be a symbol");
        }

        Symbol head = Head() as Symbol;

        LispCallable callable = scope.Callable(head);

        if (callable != null)
        {
            foreach (LispExpression exp in callable.Run(Tail().ToCSharpList(), scope))
            {
                yield return exp;
            }
        }
        else
        {
            throw new RuntimeException("Unknown callable '" + head + "'");
        }
    }

    public List<LispExpression> ToCSharpList()
    {
        return val;
    }

    public override string ToString()
    {
        string output = "(";

        for (int i=0; i<val.Count; i++)
        {
            output += val[i].ToString();
            if (i < val.Count-1)
            {
                output += " ";
            }
        }

        output += ")";

        return output;
    }

}

public class Symbol : LispExpression
{
    public string val;

    public Symbol(string val)
    {
        this.val = val.ToUpper();
    }
    
    public override IEnumerable<LispExpression> OnEvaluation(Scope scope)
    {
        yield return scope.Var(this);
    }

    public override string ToString()
    {
        return val;
    }

    public override bool Equals(object obj)
    {
        return (obj as Symbol).val == val;
    }

    public override int GetHashCode()
    {
        return val.GetHashCode();
    }

    public static bool operator ==(Symbol s1, Symbol s2)
    {
        return s1.val == s2.val;
    }

    public static bool operator !=(Symbol s1, Symbol s2)
    {
        return s1.val != s2.val;
    }
}

public class StringLiteral : LispExpression
{
    public string val;

    public StringLiteral(string val)
    {
        this.val = val;
    }

    public override IEnumerable<LispExpression> OnEvaluation(Scope scope)
    {
        yield return this;
    }

    public override bool Equals(object obj)
    {
        return (obj as StringLiteral).val == val;
    }

    public override int GetHashCode()
    {
        return val.GetHashCode();
    }

    public override string ToString()
    {
        return "\"" + val + "\"";
    }

    public static bool operator ==(StringLiteral s1, StringLiteral s2)
    {
        return s1.val == s2.val;
    }

    public static bool operator !=(StringLiteral s1, StringLiteral s2)
    {
        return s1.val != s2.val;
    }

}

public class Number : LispExpression
{
    public double val;

    public Number(double val)
    {
        this.val = val;
    }

    public override IEnumerable<LispExpression> OnEvaluation(Scope scope)
    {
        yield return this;
    }

    public override string ToString()
    {
        return val.ToString();
    }
}

public class LispBoolean : LispExpression
{
    public static LispBoolean T = new LispBoolean(true);

    public bool val;

    public LispBoolean(bool val)
    {
        this.val = val;
    }

    public override IEnumerable<LispExpression> OnEvaluation(Scope scope)
    {
        yield return this;
    }

    public override string ToString()
    {
        if (val)
        {
            return "T";
        }
        else
        {
            return "NIL";
        }
    }
}

public class LispNull : LispExpression
{
    public static LispNull NULL = new LispNull();

    public LispNull()
    {
    }

    public override IEnumerable<LispExpression> OnEvaluation(Scope scope)
    {
        yield return NULL;
    }

    public override string ToString()
    {
        return "NULL";
    }
}

public class FunctionObject : LispExpression
{
    public LispFunction val;

    public FunctionObject(LispFunction val)
    {
        this.val = val;
    }

    public override IEnumerable<LispExpression> OnEvaluation(Scope scope)
    {
        yield return this;
    }

    public override string ToString()
    {
        string output = "function object";

        return output;
    }
}
