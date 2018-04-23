using UnityEngine;
using System.Collections.Generic;
using System;

public class BookInteractionForwarder : MonoBehaviour
{
    public void OnLeftClickInteraction()
    {
        transform.parent.GetComponent<BookBehavior>().OnLeftClickInteraction();
    }

    public void OnRightClickInteraction()
    {
        transform.parent.GetComponent<BookBehavior>().OnRightClickInteraction();
    }
}
