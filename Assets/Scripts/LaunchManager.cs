using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchManager : MonoBehaviour
{
    public float SLINGSHOT_COEF = 1000f;// 5000f;
    public float InitialPlanetMass = 100.0f;
    public const float LINE_WIDTH = .2f;

    public int TopDownHeight = 100;

    public Camera defaultCamera;
    public Camera topDownCamera;
    public GameObject planetPrefab;
    public Planet scriptablePlanet;
    public GameObject potentialPlanet;
    public Vector3 launchLoc;
    public Launch_Arrow LaunchArrow;
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
        mode = Mode.NONE;
        var emptyObj = new GameObject("empty");
        scriptablePlanet = planetPrefab.GetComponent<Planet>();
    }

    void Update()
    {
        if (mode != Mode.NONE && Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
            return;
        }
        if (mode == Mode.NONE && Input.GetKeyDown(KeyCode.L))
        {
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
            switchPlanet();
        }
        if (mode == Mode.PICK_LOCATION || mode == Mode.SLINGSHOT)
        {
            Ray ray = topDownCamera.ScreenPointToRay(Input.mousePosition);
            Plane zeroPlane = new Plane(topDownCamera.transform.forward, new Vector3(0, 0, 0));
            float enter;
            if (zeroPlane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                potentialPlanet.transform.position = hitPoint;
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
        defaultCamera.gameObject.SetActive(false);
        topDownCamera.gameObject.GetComponent<AudioListener>().enabled = true;
        topDownCamera.gameObject.SetActive(true);
        topDownCamera.transform.position = new Vector3(0, TopDownHeight, 0);
        potentialPlanet = Instantiate(planetPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        potentialPlanet.name = "PreviewPlanet";
        potentialPlanet.tag = "Untagged";
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
        defaultCamera.gameObject.SetActive(true);
        topDownCamera.gameObject.SetActive(false);
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
        Destroy(potentialPlanet);
        potentialPlanet = null;
        var newPlanet = Instantiate(planetPrefab, curLoc, Quaternion.identity);
        newPlanet.name = "LaunchedPlanet";
        Rigidbody rbody = newPlanet.GetComponent<Rigidbody>();
        rbody.mass = InitialPlanetMass;
        var direction = (launchLoc - curLoc).normalized;
        var dist = (launchLoc - curLoc).magnitude;
        rbody.AddForce(direction * (float)Math.Pow(dist, 1.5f) * SLINGSHOT_COEF);
        LaunchArrow.FadeOut(0.4f);
        // Enable the gravity on the planet only once it's been launched / released:
        newPlanet.GetComponent<PlanetGravity>().enabled = true;
        var colliders = newPlanet.GetComponents<SphereCollider>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        newPlanet.transform.GetChild(0).gameObject.SetActive(true);
        // Go back to launch mode for another launch:
        StartPickLocation();
    }

    private void switchPlanet()
    {
        int i = scriptablePlanet.planetTypes.IndexOf(scriptablePlanet.planetType);
        i += 1;
        scriptablePlanet.planetType = scriptablePlanet.planetTypes[i % scriptablePlanet.planetTypes.Count];
        Destroy(potentialPlanet);
        potentialPlanet = Instantiate(planetPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        potentialPlanet.name = "PreviewPlanet";
        potentialPlanet.tag = "Untagged";
    }
}
