using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsCanvas : MonoBehaviour
{
    bool paused = false;
    public GameObject pauseCanvasObject;

    private void Awake()
    {
        pauseCanvasObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape")) {
            if (paused) {
                Unpause();
            }
            else if(SceneManager.GetActiveScene().buildIndex != 0) {
                Pause();
            }
        }
    }

    public void Pause() 
    {
        paused = true;
        Time.timeScale = 0;
        pauseCanvasObject.SetActive(true);
    }

    public void Unpause() 
    {
        paused = false;
        Time.timeScale = 1;
        pauseCanvasObject.SetActive(false);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Unpause();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene((int)GlobalManager.Scenes.MainMenu);
        Unpause();
    }
}
