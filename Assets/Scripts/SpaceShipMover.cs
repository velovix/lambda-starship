using UnityEngine;
using System.Collections.Generic;
using System;

class SpaceShipMover : MonoBehaviour
{
    public float maxLaunchThrusterVelocity;
    public float launchThrusterAcceleration;

    public GameObject universe;

    private bool launched = false;

    private Vector3 shipVelocity;

    public void Update()
    {
        if (ThrusterState.GetThrusterState(ThrusterState.LAUNCH_THRUSTER) != LispNull.NULL &&
                !launched)
        {
            launched = true;

            Shaker.Shake(10, 0.05f);

            Debug.Log("Launching");
        }

        if (launched)
        {
            if (shipVelocity.y < maxLaunchThrusterVelocity)
            {
                shipVelocity = new Vector3(
                        shipVelocity.x,
                        shipVelocity.y + (launchThrusterAcceleration * Time.deltaTime),
                        shipVelocity.z);
            }
        }

        universe.transform.position = new Vector3(
                universe.transform.position.x - (shipVelocity.x * Time.deltaTime),
                universe.transform.position.y - (shipVelocity.y * Time.deltaTime),
                universe.transform.position.z - (shipVelocity.z * Time.deltaTime));
    }
}
