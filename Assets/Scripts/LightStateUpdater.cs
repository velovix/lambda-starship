using UnityEngine;
using System.Collections.Generic;
using System;

public class LightStateUpdater : MonoBehaviour
{
    public string lightID;
    public GameObject lightSource;

    public void Start()
    {
        LightState.Add(new Symbol(lightID), LispNull.NULL);
    }

    public void Update()
    {
        if (LightState.Get(new Symbol(lightID)) != LispNull.NULL)
        {
            lightSource.SetActive(true);
        }
        else
        {
            lightSource.SetActive(false);
        }
    }
}
