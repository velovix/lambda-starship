using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class TitleButtonEvents : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
