using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class BasicInteractionManager : MonoBehaviour
{
    public Text pressE;
    public Text pressMouse;
    
    public bool showPressE = true;

    public float maxInteractionDistance;

    public void Update()
    {
        Debug.DrawRay(Camera.main.transform.position,
                Camera.main.transform.forward * maxInteractionDistance,
                Color.red,
                0.5f);

        RaycastHit[] hits = Physics.RaycastAll(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                maxInteractionDistance);

        bool facingInteractive = false;
        bool facingMouseInteractive = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Interactive")
            {
                facingInteractive = true;
                if (Input.GetKeyDown(KeyCode.E) && showPressE)
                {
                    hit.collider.SendMessage("OnInteraction");
                }
            }
            if (hit.collider.tag == "MouseInteractive")
            {
                facingMouseInteractive = true;
                if (Input.GetMouseButtonDown(0))
                {
                    hit.collider.SendMessage("OnLeftClickInteraction");
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    hit.collider.SendMessage("OnRightClickInteraction");
                }
            }
        }

        if (gameObject.active)
        {
            pressE.enabled = facingInteractive && showPressE;
            pressMouse.enabled = facingMouseInteractive;
        }
        else
        {
            pressE.enabled = false;
            pressMouse.enabled = false;
        }
    }
}
