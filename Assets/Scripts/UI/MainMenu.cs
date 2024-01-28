using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Lol c# has no string enums
public class Scenes
{
    public const string PlayScreen = "LauncherAndGravity";
    public const string MenuScreen = "TitleScreen";
}
public class MainMenu : MonoBehaviour
{
    public GameObject[] uiPanels;

    bool firstLoad = false;
    private void Awake()
    {
        //Check for first time load
        if (GameManager.singleton == null)
        {
            firstLoad = true;
            // SceneManager.LoadScene(Scenes.PlayScreen, LoadSceneMode.Single);
        }
    }

    private void Start()
    {
        // if (!PlayerPrefs.HasKey("Tutorial_Complete"))
        // {
        //     PlayerPrefs.SetInt("Tutorial_Complete", 0);
        // }

        // //They Have not completed Tutorial, Load that first
        // if (PlayerPrefs.GetInt("Tutorial_Complete") == 0 & !skipTutorialOverride)
        // {
        //     SceneManager.LoadScene(tutorialSceneName);
        // }
        // else
        // {
        //     //Load Game
        //     SelectPanel(0);
        //     GameManager.singleton.PlaySceneUnLoad();
        //     if (firstLoad)
        //     {
        //         firstLoad = false;
        //         //Play Intro
        //     }
        // }
    }


    #region Called From UI

    public void StartGame()
    {

    }

    public void SelectPanel(int panelID)
    {
        for (int i = 0; i < uiPanels.Length; i++)
        {
            uiPanels[i].SetActive(i == panelID);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion
}
