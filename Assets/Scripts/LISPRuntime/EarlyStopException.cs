using System;

public class EarlyStopException : SystemException
{
    public EarlyStopException() : base("Program was terminated early")
    {
    }
}
