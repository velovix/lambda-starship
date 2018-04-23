using System;

public class SyntaxException : SystemException
{
    public SyntaxException(string message) : base(message)
    {
    }
}

public class RuntimeException : SystemException
{
    public RuntimeException(string message) : base(message)
    {
    }
}

public class WrongArgTypeException : RuntimeException
{
    public WrongArgTypeException(string argName, Type desired, Type actual) :
        base("Argument " + argName + " must be of type " + desired + ", got " + actual)
    {
    }
}
