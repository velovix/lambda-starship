using UnityEngine;
using System.Collections.Generic;
using System;

public class BookBehavior : MonoBehaviour
{
    public GameObject cover;
    public GameObject page;
    public GameObject interactable;

    public GameObject uprightDestination;

    public List<Texture> pages;
    public GameObject pageCover;

    private Renderer renderer;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private int pageNumber = -1;

    private bool upright = false;

    void Start()
    {
        initialPosition = cover.transform.position;
        initialRotation = cover.transform.rotation;
        renderer = pageCover.GetComponent<Renderer>();
    }

    void Update()
    {
        if (pageNumber == -1)
        {
            page.SetActive(false);
            cover.SetActive(true);
        }
        else
        {
            page.SetActive(true);
            cover.SetActive(false);
            renderer.material.mainTexture = pages[pageNumber];
        }

        if (upright)
        {
            cover.transform.position = uprightDestination.transform.position;
            cover.transform.rotation = uprightDestination.transform.rotation;
            page.transform.position = uprightDestination.transform.position;
            page.transform.rotation = uprightDestination.transform.rotation;
            interactable.transform.position = uprightDestination.transform.position;
            interactable.transform.rotation = uprightDestination.transform.rotation;
        }
        else
        {
            cover.transform.position = initialPosition;
            cover.transform.rotation = initialRotation;
            page.transform.position = initialPosition;
            page.transform.rotation = initialRotation;
            interactable.transform.position = initialPosition;
            interactable.transform.rotation = initialRotation;
        }
    }

    public void SetUpright(bool upright)
    {
        this.upright = upright;
        interactable.SetActive(upright);
    }

    public void OnLeftClickInteraction()
    {
        if (pageNumber < pages.Count-1)
        {
            pageNumber++;
        }
    }

    public void OnRightClickInteraction()
    {
        if (pageNumber > -1)
        {
            pageNumber--;
        }
    }
}
