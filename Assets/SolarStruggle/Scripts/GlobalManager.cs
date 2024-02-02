using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance { get; private set; }

    public bool paused;
    bool gameIsOver = false;

    // TODO: move UI functionality to another class
    public TMP_Text pausedText;
    public GameObject planetText;
    public Canvas screenCanvas;
    public LaunchManager launchManager;
    public Vector3 prevMousePos;

    private GlobalManager()
    {

    }

    private void Awake()
    {
        // If there is an instance and it's not me, delete myself
        if (Instance is not null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);

            // TODO: move this to main menu handling if even necessary
            var mmMusic = GameObject.Find("MainMenuMusic");
            if (mmMusic != null)
            {
                mmMusic.GetComponent<MainMenuMusic>().StopMusic();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Resume();
        launchManager = GameObject.Find("LaunchManager").GetComponent<LaunchManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameIsOver && Input.GetKeyDown(KeyCode.Escape))
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
                (int)Scenes.MainMenu,
                LoadSceneMode.Single);
            gameIsOver = false;
        }
    }

    private void FixedUpdate()
    {
        if (launchManager.mode == LaunchManager.Mode.NONE || launchManager.mode == LaunchManager.Mode.PICK_LOCATION)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, Mathf.Infinity);
            var planetHits = new List<RaycastHit>();
            foreach (var hit in hits)
            {
                if (hit.transform.gameObject.tag == "Planet")
                {
                    planetHits.Add(hit);
                }
            }
            if (planetHits.Count > 0)
            {
                var hit = planetHits.OrderBy(hit => (Camera.main.WorldToScreenPoint(hit.transform.position) - Input.mousePosition).magnitude).First();
                string planetName = hit.transform.gameObject.GetComponent<Planet>().planetName;
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(hit.transform.position);
                var textPoint = new Vector2(screenPoint.x, screenPoint.y + 50);
                Vector2 canvasPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(screenCanvas.GetComponent<RectTransform>(), textPoint, null, out canvasPos);
                planetText.GetComponent<RectTransform>().localPosition = canvasPos;
                planetText.GetComponent<TMP_Text>().text = planetName;
                planetText.GetComponent<TMP_Text>().enabled = true;
            }
            else
            {
                planetText.GetComponent<TMP_Text>().enabled = false;
            }
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

    public enum Scenes : int
    {
        MainMenu = 0,
        Play = 1,
        Credits = 2,
        Tutorial = 3,
    }
}
