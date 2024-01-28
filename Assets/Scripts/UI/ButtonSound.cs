using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public GameObject arrow;
    public void ButtonHover()
    {
        // GameManager.PlayButtonSound(0);
        arrow.GetComponent<Image>().enabled = true;
    }

    public void ButtonUnhover()
    {
        // GameManager.PlayButtonSound(0);
        arrow.GetComponent<Image>().enabled = false;
    }

    public void ButtonClickPlay()
    {
        // GameManager.PlayButtonSound(1);
        SceneManager.LoadScene(GameManager.GetSceneNameFromEnum(Scenes.TutorialFirst), LoadSceneMode.Single);
    }

    public void ButtonClickQuit()
    {
        // GameManager.PlayButtonSound(1);
        Debug.Log("quit");
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        } 
        else
        {
            Application.Quit();
        }
    }
}