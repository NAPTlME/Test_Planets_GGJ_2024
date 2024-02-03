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
    public CameraManager cameraManager;
    public Transform SizzleParticleTransform;
    private ParticleSystem _sizzleParticleSystem;
    public float SizzleOffset = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        Resume();
        launchManager = GameObject.Find("LaunchManager").GetComponent<LaunchManager>();
        _sizzleParticleSystem = SizzleParticleTransform.gameObject.GetComponentInChildren<ParticleSystem>();
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
        var mmMusic = GameObject.Find("MainMenuMusic");
        if (mmMusic != null)
        {
            mmMusic.GetComponent<MainMenuMusic>().StopMusic();
        }
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
    public void SizzleAt(ContactPoint contact)
    {
        /*// set rotation normal to the sun
        var normalDir = contact.point - GravityManager.getInstance().gravityPlanets.First().transform.position;
        // set position for emit
        SizzleParticleTransform.position = contact.point + normalDir.normalized * SizzleOffset;
        //SizzleParticleTransform.rotation = SizzleParticleTransform.rotation * Quaternion.FromToRotation(SizzleParticleTransform.TransformDirection(Vector3.up), normalDir);
        //SizzleParticleTransform.rotation = Quaternion.FromToRotation(SizzleParticleTransform.TransformDirection(Vector3.up), normalDir);
        // rotate based on camera position?
        var cameraDir = cameraManager.mainCamera.transform.position - contact.point;
        if (Mathf.Abs(Vector3.Dot(normalDir, cameraDir)) != 1)
        {
            // not parallel to normal, can project
            var cameraDirOnNormalPlane = Vector3.ProjectOnPlane(cameraDir, normalDir);
            var signedAngle = Vector3.SignedAngle(SizzleParticleTransform.forward, cameraDirOnNormalPlane, SizzleParticleTransform.up);
            //SizzleParticleTransform.rotation = SizzleParticleTransform.rotation * Quaternion.AngleAxis(signedAngle, SizzleParticleTransform.up);
            SizzleParticleTransform.rotation = Quaternion.LookRotation(cameraDirOnNormalPlane, normalDir);
        }
        _sizzleParticleSystem.Emit(1);*/

        var normalDir = contact.point - GravityManager.getInstance().gravityPlanets.First().transform.position;
        // set position for emit
        var particleParam = new ParticleSystem.EmitParams();
        particleParam.position = contact.point + normalDir.normalized * SizzleOffset;
        var cameraDir = cameraManager.mainCamera.transform.position - contact.point;
        var cameraDirOnNormalPlane = Vector3.ProjectOnPlane(cameraDir, normalDir);

        // I wasted 3 hours on this negative quaternion thing...
        // https://forum.unity.com/threads/particle-rotation-always-wrong-on-one-axis-when-using-emit.985989/
        particleParam.rotation3D = -Quaternion.LookRotation(cameraDirOnNormalPlane, normalDir).eulerAngles;
        //particleParam.rotation3D = Quaternion.FromToRotation(Vector3.up, )
        Debug.DrawRay(contact.point, cameraDirOnNormalPlane, Color.green, 3f);
        Debug.DrawRay(contact.point, normalDir, Color.blue, 3f);
        _sizzleParticleSystem.Emit(particleParam, 1);
    }
}
