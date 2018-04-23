using UnityEngine;
using System.Collections;
using System;

public class Shaker : MonoBehaviour
{
    private static DateTime start;
    private static int duration;
    private static float intensity;

    private Vector3 originalPosition;
    private Vector3 lastShakeAmount;

    public void Start()
    {
        start = DateTime.Now;
        duration = 0;
        originalPosition = transform.position;
    }

    public void Update()
    {
        originalPosition = transform.position - lastShakeAmount;

        if (DateTime.Now < start.AddSeconds(duration))
        {
            lastShakeAmount = UnityEngine.Random.insideUnitSphere * intensity;

            transform.position = originalPosition + lastShakeAmount;
        }
    }

    public static void Shake(int duration, float intensity)
    {
        start = DateTime.Now;
        Shaker.duration = duration;
        Shaker.intensity = intensity;
    }
}
