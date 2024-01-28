using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public bool paused;
    private static GlobalManager instance = null;
    public TMP_Text pausedText;
    public GameObject planetText;
    public Canvas screenCanvas;
    bool gameIsOver = false;
    public LaunchManager launchManager;
    public Vector3 prevMousePos;
    // Start is called before the first frame update
    void Start()
    {
        paused = false;
        launchManager = GameObject.Find("LaunchManager").GetComponent<LaunchManager>();
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
}
