using UnityEngine;
using System.Collections.Generic;
using System;

public class UUIDState
{
    private static string uuid;

    static UUIDState()
    {
        uuid = UnityEngine.Random.Range(0, 999999999).ToString();
        Debug.Log("This run has a UUID of " + uuid);
    }

    public static string Get()
    {
        return uuid;
    }
}
