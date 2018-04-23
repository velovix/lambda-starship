using UnityEngine;
using System.Collections.Generic;
using System;

public class WarpDriveLeverBehaviour : MonoBehaviour
{
    public GameObject handle;

    public float offRotationDelta;
    public float onRotationDelta;
    public float rotationSpeed = 4.0f;

    private float initialRotation;
    private float lastRotation;
    private float targetRotation;

    void Start()
    {
        initialRotation = handle.transform.eulerAngles.z;

        if (WarpDriveLeverState.Get() == LispNull.NULL)
        {
            SetZRotation(offRotationDelta + initialRotation);
        }
        else
        {
            SetZRotation(onRotationDelta + initialRotation);
        }

        targetRotation = handle.transform.eulerAngles.z;
    }

    void Update()
    {
        if (WarpDriveLeverState.Get() == LispNull.NULL)
        {
            lastRotation = handle.transform.eulerAngles.z;
            targetRotation = initialRotation + offRotationDelta;
        }
        else
        {
            lastRotation = handle.transform.eulerAngles.z;
            targetRotation = initialRotation + onRotationDelta;
        }

        float z = Mathf.LerpAngle(
                lastRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        SetZRotation(z);
    }

    void SetZRotation(float z)
    {
        handle.transform.eulerAngles = new Vector3(
                handle.transform.eulerAngles.x,
                handle.transform.eulerAngles.y,
                z);
    }

    public void OnInteraction()
    {
        WarpDriveLeverState.Toggle();
    }
}
