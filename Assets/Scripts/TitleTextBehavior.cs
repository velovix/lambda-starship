using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class TitleTextBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const string RANDOM_CHARS = "!@#$%^&*+-_\\/";

    public string neutralText;
    public string mouseOverText;

    private bool transitioning;
    private int index;
    private string currText;

    private Text text;

    public void Start()
    {
        text = GetComponent<Text>();

        currText = neutralText;
    }

    public void Update()
    {
        if (transitioning)
        {
            if (index < mouseOverText.Length)
            {
                char randChar = RANDOM_CHARS[UnityEngine.Random.Range(0, RANDOM_CHARS.Length)];
                currText = mouseOverText.Substring(0, index) + randChar;
                if (currText.Length < neutralText.Length)
                {
                    currText += neutralText.Substring(index+1);
                }

                index++;
            }
            else
            {
                currText = mouseOverText;
            }
        }
        else
        {
            currText = neutralText;
        }

        text.text = currText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transitioning = true;
        index = 0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transitioning = false;
    }
}
