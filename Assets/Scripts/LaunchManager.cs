using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchManager : MonoBehaviour
{
    public const float SLINGSHOT_COEF = 5000f;
    public const float LINE_WIDTH = .2f;

    public Camera defaultCamera;
    public Camera topDownCamera;
    public GameObject planetPrefab;
    public GameObject potentialPlanet;
    public Vector3 launchLoc;
    public LineRenderer lineRenderer;
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
        lineRenderer = emptyObj.AddComponent<LineRenderer>();
        lineRenderer.startWidth = LINE_WIDTH;
        lineRenderer.endWidth = 0f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 0;
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
            lineRenderer.SetPositions(trajectory.ToArray());
        }
    }

    private void StartPickLocation()
    {
        Debug.Assert(mode == Mode.NONE || mode == Mode.SLINGSHOT);
        mode = Mode.PICK_LOCATION;
        defaultCamera.gameObject.SetActive(false);
        topDownCamera.gameObject.SetActive(true);
        topDownCamera.transform.position = new Vector3(0, 20, 0);
        potentialPlanet = Instantiate(planetPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        potentialPlanet.tag = "Untagged";
        Destroy(potentialPlanet.GetComponent<Rigidbody>());
        var colliders = potentialPlanet.GetComponents<Collider>();
        foreach(var collider in colliders)
        {
            Destroy(collider);
        }
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
        lineRenderer.positionCount = 0;
    }

    private void StartSlingshot()
    {
        Debug.Assert(mode == Mode.PICK_LOCATION);
        mode = Mode.SLINGSHOT;
        launchLoc = potentialPlanet.transform.position;
        lineRenderer.positionCount = 2;
    }

    private void Launch()
    {
        Debug.Assert(mode == Mode.SLINGSHOT);
        var curLoc = potentialPlanet.transform.position;
        Destroy(potentialPlanet);
        potentialPlanet = null;
        var newPlanet = Instantiate(planetPrefab, curLoc, Quaternion.identity);
        Rigidbody rbody = newPlanet.GetComponent<Rigidbody>();
        var direction = (launchLoc - curLoc).normalized;
        var dist = (launchLoc - curLoc).magnitude;
        rbody.AddForce(direction * (float)Math.Pow(dist, 1.5f) * SLINGSHOT_COEF);
        lineRenderer.positionCount = 0;
        StartPickLocation();
    }
}