using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Lol c# has no string enums

public class MainMenu : MonoBehaviour
{
    public GameObject[] uiPanels;

    private void Awake()
    {

    }

    private void Start()
    {
        
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
