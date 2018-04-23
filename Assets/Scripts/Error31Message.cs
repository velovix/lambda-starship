using UnityEngine;
using System.Collections.Generic;
using System;

public class Error31Message
{
    private const string MESSAGE = "ERROR 31: Software is missing from device and all backups appear to be corrupted. Please see the corresponding manual for details.\n\n\nPress ENTER to continue.";

    public Error31Message()
    {

    }

    public string GetContent()
    {
        return MESSAGE;
    }
}
