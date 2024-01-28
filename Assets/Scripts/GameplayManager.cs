using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public bool paused;
    public TMP_Text pausedText;
    public GameObject planetText;
    public Canvas screenCanvas;
    public LaunchManager launchManager;
    public Vector3 prevMousePos;
    // Start is called before the first frame update
    void Start()
    {
        paused = false;
        launchManager = GameObject.Find("LaunchManager").GetComponent<LaunchManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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
        if (launchManager.mode == LaunchManager.Mode.NONE)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, Mathf.Infinity);
            if (hits.Length > 0)
            {
                var hit = hits.OrderBy(hit => (Camera.main.WorldToScreenPoint(hit.transform.position) - Input.mousePosition).magnitude).First();
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
