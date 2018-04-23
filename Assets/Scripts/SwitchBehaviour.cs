using UnityEngine;
using System.Collections.Generic;
using System;

public class SwitchBehaviour : MonoBehaviour
{
    public string switchID;
    public GameObject switchModel;

    private Symbol lispSwitchID;
    private Vector3 initialSwitchRotation;

    public void Start()
    {
        lispSwitchID = new Symbol(switchID);
        SwitchState.Add(lispSwitchID, LispNull.NULL);
        initialSwitchRotation = switchModel.transform.eulerAngles;
    }

    public void OnInteraction()
    {
        SwitchState.Toggle(lispSwitchID);
    }

    public void Update()
    {
        if (SwitchState.Get(lispSwitchID) != LispNull.NULL)
        {
            switchModel.transform.eulerAngles = initialSwitchRotation;
        }
        else
        {
            switchModel.transform.eulerAngles = new Vector3(
                    initialSwitchRotation.x,
                    initialSwitchRotation.y,
                    initialSwitchRotation.z + 32);
        }
    }

}
