using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class LaunchManager : MonoBehaviour
{
    public float SLINGSHOT_COEF;// 5000f;
    public const float LINE_WIDTH = .2f;

    public int TopDownHeight = 100;

    private Planet previewPlanet;
    private Vector3 launchLoc;
    public Launch_Arrow LaunchArrow;
    private CameraManager cameraManager;
    public GameObject boing;

    int planetTypeInteractionIndex = 0;

    [Header("Planet Builder")]
    public DodecPlanetBuild planetBuilder;

    public enum Mode
    {
        NONE,
        PICK_LOCATION,
        SLINGSHOT
    }
    public Mode mode;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        mode = Mode.PICK_LOCATION;
        var emptyObj = new GameObject("empty");

        cameraManager = Camera.main.gameObject.GetComponent<CameraManager>();
    }

    void Update()
    {
        if (mode != Mode.NONE && Input.GetKeyDown(KeyCode.Space))
        {
            cameraManager.SwapCameras();
            Exit();
            return;
        }
        if (mode == Mode.NONE && Input.GetKeyDown(KeyCode.Space))
        {
            cameraManager.SwapCameras();
            StartPickLocation(destroyPreviousPlanet: true);
            return;
        }
        if (mode == Mode.PICK_LOCATION && Input.GetMouseButtonDown(0)) // left click
        {
            StartSlingshot();
            return;
        }
        if (mode == Mode.SLINGSHOT && Input.GetMouseButtonUp(0))
        {
            Launch();
            return;
        }
        if (mode != Mode.NONE && (Input.GetKeyDown(KeyCode.Tab) || Input.GetMouseButtonDown(1)))
        {
            planetTypeInteractionIndex += 1;
            newPotentialPlanet(destroyPrevious: true);
        }
        if (mode == Mode.PICK_LOCATION || mode == Mode.SLINGSHOT)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane zeroPlane = new Plane(Camera.main.transform.forward, new Vector3(0, 0, 0));
            float enter;

            if (zeroPlane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                if (previewPlanet == null)
                {
                    newPotentialPlanet(destroyPrevious: false);
                }

                // amt to move planet
                var planetMoveDelta = hitPoint - previewPlanet.transform.position;
                previewPlanet.transform.position = hitPoint;

                // also move any child rigidbodies
                var childRigidBodies = previewPlanet.GetComponentsInChildren<Rigidbody>().Skip(1).ToList();
            }
        }
        if (mode == Mode.SLINGSHOT)
        {
            var trajectory = new List<Vector3>()
            {
                previewPlanet.transform.position,
                launchLoc
            };
            Vector2 launchLoc_2d = new Vector2(launchLoc.x, launchLoc.z);
            Vector2 potentialPlanetPos_2d = new Vector2(previewPlanet.transform.position.x, previewPlanet.transform.position.z);
            LaunchArrow.UpdatePosition(launchLoc_2d, potentialPlanetPos_2d);
        }
    }

    private void StartPickLocation(bool destroyPreviousPlanet)
    {
        Debug.Assert(mode == Mode.NONE || mode == Mode.SLINGSHOT);
        mode = Mode.PICK_LOCATION;

        newPotentialPlanet(destroyPreviousPlanet);
    }

    private void Exit()
    {
        Debug.Assert(mode != Mode.NONE);
        mode = Mode.NONE;



        if (previewPlanet != null)
        {
            Destroy(previewPlanet.gameObject);
        }
        LaunchArrow.enabled = false;
    }

    private void StartSlingshot()
    {
        Debug.Assert(mode == Mode.PICK_LOCATION);
        mode = Mode.SLINGSHOT;
        launchLoc = previewPlanet.transform.position;
        LaunchArrow.enabled = true;
        LaunchArrow.SetFade(0f);
        LaunchArrow.FadeIn(0.2f);
    }

    private void Launch()
    {
        Debug.Assert(mode == Mode.SLINGSHOT);
        var curLoc = previewPlanet.transform.position;
        var launchedPlanet = previewPlanet;

        launchedPlanet.name = "LaunchedPlanet";

        Rigidbody rbody = launchedPlanet.GetComponent<Rigidbody>();
        var direction = (launchLoc - curLoc).normalized;
        var dist = (launchLoc - curLoc).magnitude;
        rbody.AddForce(direction * dist * SLINGSHOT_COEF * rbody.mass);

        LaunchArrow.FadeOut(0.4f);

        // Enable the gravity on the planet only once it's been launched / released:
        launchedPlanet.GetComponent<PlanetGravity>().enabled = true;
        var colliders = launchedPlanet.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        launchedPlanet.SetTrailRendererEnabled(true);

        launchedPlanet.tag = "Planet";

        //// find all entitys and set home (for some reason it is not setting)
        //var childEntities = launchedPlanet.GetComponentsInChildren<Objects_On_Planet>();
        //Debug.Log("num children entities: " + childEntities.Count());

        //var planetObj = newPlanet.GetComponent<Planet>();
        //childEntities.ToList().ForEach(x => x.homePlanet = planetObj);
        //newPlanet.transform.GetComponentInChildren<TrailRenderer>().gameObject.SetActive(true); // todo this doesn't seem to be working.

        StatsManager.Instance.PlanetLaunched(previewPlanet.planetType);
        cameraManager.SetFocusTarget(launchedPlanet.transform);

        boing.GetComponent<AudioSource>().Play();
        // Go back to launch mode for another launch:
        StartPickLocation(destroyPreviousPlanet: false);
    }

    private void newPotentialPlanet(bool destroyPrevious)
    {
        // Destroy the prior preview planet. Good for planet and camera swapping
        if(destroyPrevious && previewPlanet)
        {
            Destroy(previewPlanet.gameObject);
        }
        previewPlanet = null;
        
        var newPlanetType = planetBuilder.scriptable_planets[planetTypeInteractionIndex % planetBuilder.scriptable_planets.Count];
        var newPlanet = planetBuilder.Build(newPlanetType);

        previewPlanet = newPlanet;
        previewPlanet.name = "PreviewPlanet";
        previewPlanet.tag = "Untagged";
        previewPlanet.GetComponentsInChildren<Collider>().ToList().ForEach(sel => sel.enabled = false);
    }
}
