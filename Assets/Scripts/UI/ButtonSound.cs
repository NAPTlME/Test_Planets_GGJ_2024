using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSound : MonoBehaviour
{
    public void ButtonHover()
    {
        // GameManager.PlayButtonSound(0);
    }

    public void ButtonClickPlay()
    {
        // GameManager.PlayButtonSound(1);
        SceneManager.LoadScene(Scenes.PlayScreen, LoadSceneMode.Single);
    }

    public void ButtonClickQuit()
    {
        // GameManager.PlayButtonSound(1);
        // Application.Quit();

    }
}
