using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OptionsMenu()
    {
        Debug.Log("Options Menu");
    }

    public void QuitGame()
    {
        Application.Quit(0);
        Debug.Log("Quitting Game");
    }
}
