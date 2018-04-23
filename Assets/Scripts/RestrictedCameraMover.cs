using UnityEngine;
using System.Collections.Generic;
using System;

public class RestrictedCameraMover : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void Update()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        float diff = transform.eulerAngles.y - initialRotation.eulerAngles.y;

        if ((x > 0.0f && (diff < 90 || diff > 180)) || 
            (x < 0.0f && (diff > 270 || diff < 180)))
        {
            transform.eulerAngles = new Vector3(
                    transform.eulerAngles.x,
                    transform.eulerAngles.y + x,
                    transform.eulerAngles.z);
        }
    }

    public void Reset()
    {
        transform.rotation = initialRotation;
    }
}
