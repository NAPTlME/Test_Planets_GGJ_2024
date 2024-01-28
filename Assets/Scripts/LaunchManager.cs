using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class LaunchManager : MonoBehaviour
{
    public float SLINGSHOT_COEF = 1000f;// 5000f;
    public float InitialPlanetMass = 100.0f;
    public const float LINE_WIDTH = .2f;

    public int TopDownHeight = 100;

    public GameObject planetPrefab;

    public Planet currentPlanet;
    private GameObject potentialPlanet;
    private Vector3 launchLoc;
    public Launch_Arrow LaunchArrow;
    private StatsPlanetType statsPlanetType = StatsPlanetType.SMALL;
    private CameraManager cameraManager;

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

        cameraManager = Camera.main.gameObject.GetComponent<CameraManager>();
    }

    void Update()
    {
        if (mode != Mode.NONE && Input.GetKeyDown(KeyCode.Escape))
        {
            cameraManager.SwapCameras();
            Exit();
            return;
        }
        if (mode == Mode.NONE && Input.GetKeyDown(KeyCode.L))
        {
            cameraManager.SwapCameras();
            StartPickLocation();
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
        if (mode != Mode.NONE && Input.GetKeyDown(KeyCode.Tab))
        {
            planetTypeInteractionIndex += 1;
            newPotentialPlanet();
        }
        if (mode == Mode.PICK_LOCATION || mode == Mode.SLINGSHOT)
        {
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
                var planetMoveDelta = hitPoint - potentialPlanet.transform.position;
                potentialPlanet.transform.position = hitPoint;
                // also move any child rigidbodies
                var childRigidBodies = potentialPlanet.GetComponentsInChildren<Rigidbody>().Skip(1).ToList();
                //childRigidBodies.ForEach(x => x.transform.position += planetMoveDelta);
                //Debug.Log("number of child rigidbodies: " + childRigidBodies.Count);
            }
        }
        if (mode == Mode.SLINGSHOT)
        {
            var trajectory = new List<Vector3>()
            {
                potentialPlanet.transform.position,
                launchLoc
            };
            Vector2 launchLoc_2d = new Vector2(launchLoc.x, launchLoc.z);
            Vector2 potentialPlanetPos_2d = new Vector2(potentialPlanet.transform.position.x, potentialPlanet.transform.position.z);
            LaunchArrow.UpdatePosition(launchLoc_2d, potentialPlanetPos_2d);
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
        // TODO: We could avoid detroying the temp planet and creating a new one
        // if we wanted... later
        Debug.Assert(mode == Mode.SLINGSHOT);
        var curLoc = potentialPlanet.transform.position;
        var newPlanet = potentialPlanet;
        potentialPlanet = null;
        newPlanet.name = "LaunchedPlanet";
        Rigidbody rbody = newPlanet.GetComponent<Rigidbody>();
        var direction = (launchLoc - curLoc).normalized;
        var dist = (launchLoc - curLoc).magnitude;
        rbody.AddForce(direction * (float)Math.Pow(dist, 1.5f) * SLINGSHOT_COEF);
        LaunchArrow.FadeOut(0.4f);
        // Enable the gravity on the planet only once it's been launched / released:
        newPlanet.GetComponent<PlanetGravity>().enabled = true;
        var colliders = newPlanet.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        newPlanet.GetComponent<Planet>().SetTrailRendererEnabled(true);
        newPlanet.tag = "Planet";

        // find all entitys and set home (for some reason it is not setting)
        var childEntities = newPlanet.GetComponentsInChildren<Objects_On_Planet>();
        Debug.Log("num children entities: " + childEntities.Count());
        //var planetObj = newPlanet.GetComponent<Planet>();
        //childEntities.ToList().ForEach(x => x.homePlanet = planetObj);
        //newPlanet.transform.GetComponentInChildren<TrailRenderer>().gameObject.SetActive(true); // todo this doesn't seem to be working.

        StatsManager.getInstance().PlanetLaunched(currentPlanet.planetType);
        cameraManager.SetFocusTarget(newPlanet.transform);
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
