using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
    private static GlobalManager instance = null;
    public bool paused;
    bool gameIsOver = false;
    public TMP_Text pausedText;
    // Start is called before the first frame update
    void Start()
    {
        paused = false;
    }
    public static GlobalManager getInstance()
    {
        return instance;
    }
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameIsOver && Input.GetKeyDown(KeyCode.Space))
        {
            if (!paused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
        else if (gameIsOver && Input.anyKey)
        {
            SceneManager.LoadScene(
                GameManager.GetSceneNameFromEnum(Scenes.MenuScreen),
                LoadSceneMode.Single);
        }
    }

    public void GameOver(int score)
    {
        gameIsOver = true;
        Pause();
        pausedText.text = "Game Over";
    }
    private void Pause()
    {
        paused = true;
        Time.timeScale = 0;
        pausedText.enabled = true;
    }

    private void Resume()
    {
        paused = false;
        Time.timeScale = 1;
        pausedText.enabled = false;
    }
}
