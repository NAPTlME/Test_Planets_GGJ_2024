using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class LaunchManager : MonoBehaviour
{
    public float SLINGSHOT_COEF = 1000f;// 5000f;
    public const float LINE_WIDTH = .2f;

    public int TopDownHeight = 100;

    public GameObject planetPrefab;

    public Planet currentPlanet;
    private GameObject potentialPlanet;
    private Vector3 launchLoc;
    public Launch_Arrow LaunchArrow;
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
        currentPlanet = planetPrefab.GetComponent<Planet>();
    }

    void Update()
    {
        switch(mode)
        {
            case Mode.NONE:
                HandleModeNone();
                break;
            case Mode.PICK_LOCATION:
                HandleModePickLocation();
                break;
            case Mode.SLINGSHOT:
                HandleModeSlingshot();
                break;
        }
        HandlePlanetCameraCycle();
    }

    private void HandleModeNone()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GlobalManager.getInstance().cameraManager.SwapCameras();
            StartPickLocation();
        }
    }
    private void HandleModePickLocation()
    {
        if (HandleLauncherActiveLogic_Before_ShouldContinue())
        {
            if (Input.GetMouseButtonDown(0)) // left click
            {
                StartSlingshot();
            }
            HandleLauncherActiveLogic_After();
        }
    }
    private void HandleModeSlingshot()
    {
        if (HandleLauncherActiveLogic_Before_ShouldContinue())
        {
            if (Input.GetMouseButtonUp(0))
            {
                Launch();
            }
            HandleLauncherActiveLogic_After();
            Vector2 launchLoc_2d = new Vector2(launchLoc.x, launchLoc.z);
            Vector2 potentialPlanetPos_2d = new Vector2(potentialPlanet.transform.position.x, potentialPlanet.transform.position.z);
            LaunchArrow.UpdatePosition(launchLoc_2d, potentialPlanetPos_2d);
        }
    }
    private void HandlePlanetCameraCycle()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GravityManager.getInstance().CyclePlanetCam(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            GravityManager.getInstance().CyclePlanetCam(1);
        }
    }
    private bool HandleLauncherActiveLogic_Before_ShouldContinue()
    {
        // allows for an exit from a launcher state
        var continueUpdate = true;
        if (Input.GetKeyDown(KeyCode.L))
        {
            GlobalManager.getInstance().cameraManager.SwapCameras();
            Exit();
            continueUpdate = false;
        }
        return continueUpdate;
    }
    private void HandleLauncherActiveLogic_After()
    {
        // changing potential planet
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            planetTypeInteractionIndex += 1;
            newPotentialPlanet();
        }
        // setting position for cursor (potential) planet
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane zeroPlane = new Plane(Camera.main.transform.forward, new Vector3(0, 0, 0));
        float enter;
        if (zeroPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            if (potentialPlanet == null)
            {
                newPotentialPlanet();
            }
            // amt to move planet
            potentialPlanet.transform.position = hitPoint;

        }
    }

    private void StartPickLocation()
    {
        Debug.Assert(mode == Mode.NONE || mode == Mode.SLINGSHOT);
        mode = Mode.PICK_LOCATION;

        newPotentialPlanet();
        // Note: if what we want is the colliders to be disabled in the initial state,
        // what we should do is have them disabled in the prefab and then enabling them on
        // Launch() like for the planetGravity component, so commenting out this code for
        // now until we can clarify its goal
        // Destroy(potentialPlanet.GetComponent<Rigidbody>());
        // var colliders = potentialPlanet.GetComponents<Collider>();
        // foreach (var collider in colliders)
        // {
        //     Destroy(collider);
        // }
    }

    private void Exit()
    {
        Debug.Assert(mode != Mode.NONE);
        mode = Mode.NONE;



        if (potentialPlanet != null)
        {
            Destroy(potentialPlanet);
        }
        LaunchArrow.enabled = false;
    }

    private void StartSlingshot()
    {
        Debug.Assert(mode == Mode.PICK_LOCATION);
        mode = Mode.SLINGSHOT;
        launchLoc = potentialPlanet.transform.position;
        LaunchArrow.enabled = true;
        LaunchArrow.SetFade(0f);
        LaunchArrow.FadeIn(0.2f);
    }

    private void Launch()
    {
        Debug.Assert(mode == Mode.SLINGSHOT);
        var curLoc = potentialPlanet.transform.position;
        var planetObj = potentialPlanet;
        var newPlanet = planetObj.GetComponent<Planet>();
        potentialPlanet = null;
        newPlanet.name = "LaunchedPlanet_" + newPlanet.planetName;
        Rigidbody rbody = newPlanet.orbitalPlanetObj.GetComponent<Rigidbody>();
        var direction = (launchLoc - curLoc).normalized;
        var dist = (launchLoc - curLoc).magnitude;
        rbody.AddForce(direction * (float)Math.Pow(dist, 1.5f) * SLINGSHOT_COEF * rbody.mass);
        LaunchArrow.FadeOut(0.4f);
        // Enable the gravity on the planet only once it's been launched / released:
        var planetGrav = newPlanet.orbitalPlanetObj.GetComponent<PlanetGravity>();
        planetGrav.enabled = true;
        var colliders = newPlanet.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        var rbodyEntities = newPlanet.localPlanetObj.GetComponentsInChildren<Objects_On_Planet>();
        rbodyEntities.ToList().ForEach(x =>
        {
            x.SetCanBeKilled(0.01f);
            x.SetHomePlanet(newPlanet);
        });
        newPlanet.SetTrailRendererEnabled(true);
        newPlanet.tag = "Planet";
        planetGrav.Launched();

        StatsManager.getInstance().PlanetLaunched(currentPlanet.planetType);
        GravityManager.getInstance().SetActivePlanetCam(planetGrav);

        boing.GetComponent<AudioSource>().Play();
        // Go back to launch mode for another launch:
        StartPickLocation();
    }

    private void newPotentialPlanet()
    {
        if (potentialPlanet != null)
        {
            Destroy(potentialPlanet);
            potentialPlanet = null;
        }
        var newPlanetType = planetBuilder.scriptable_planets[planetTypeInteractionIndex % planetBuilder.scriptable_planets.Count];
        var newPlanet = planetBuilder.Build(newPlanetType);
        potentialPlanet = newPlanet.Item1;
        potentialPlanet.name = "PreviewPlanet";
        potentialPlanet.tag = "Untagged";
        potentialPlanet.GetComponentsInChildren<Collider>().ToList().ForEach(sel => sel.enabled = false);
        currentPlanet.planetType = newPlanet.Item2;
    }
}
