using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Cinemachine;

public class GravityManager : MonoBehaviour
{

    private static GravityManager instance = null;
    public double MaxDistanceBeforeBending = 90f; // checked when it seemed "too far"
    public double MaxDistanceBeforeLost = 200f;
    public float BendingForce = 100f;

    public float PullBackToPerpendicularRatio = 5f;
    // Not a real constant so we can use Double.Parse()
    static private double GRAVITY = 1;

    // This does not need to be public but it makes inspecting it in the inspector for debugging
    // quite convenient
    public List<PlanetGravity> gravityPlanets = new List<PlanetGravity>();
    public Action<PlanetGravity> OnActivePlanetChanged;
    public static GravityManager getInstance()
    {
        return instance;
    }

    public void RegisterPlanet(PlanetGravity planetGravity)
    {
        gravityPlanets.Add(planetGravity);
    }

    public void ForgetPlanet(PlanetGravity planetGravity)
    {
        // TODO: This is inefficient, we should look into using sets, but IDK C#'s default hashing requirements
        // and there won't be many planets so this is good enough for now
        gravityPlanets.Remove(planetGravity);
    }


    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    private void FixedUpdate()
    {
        // TODO: Later, consider halving the second loop for optimization
        // todo: for ij
        foreach (var planetGravity in gravityPlanets)
        {
            foreach (var planetGravityOther in gravityPlanets)
            {
                if (planetGravityOther == planetGravity)
                {
                    continue;
                }
                var direction = planetGravityOther.transform.position - planetGravity.transform.position;
                var distance = direction.magnitude;
                var force = GRAVITY * (planetGravity.rigidBody.mass * planetGravityOther.rigidBody.mass) / (distance * distance);
                planetGravity.rigidBody.AddForce(direction / distance * ((float)force), ForceMode.Force);
            }

        }
    }

    public void CyclePlanetCam(int x)
    {
        Debug.Log("CyclePlanet: " + "+ " + x);
        if (gravityPlanets.Count > 0)
        {
            // only intend to use x as -1|1, but factoring in other potential uses (in case a scroll wheel is used and the value can be more than 1
            if (x + gravityPlanets.Count <= 0) // handle large negative numbers as no movement
            {
                x = 0;
            }
            // get which index has the highest priority (10 vs 11) 
            var activeIndex = gravityPlanets.Select((planet, i) => (i, planet.vCamera.Priority)).OrderByDescending(x => x.Priority).Select(x => x.i).First();
            Debug.Log("Active Index: " + activeIndex);
            activeIndex += x;
            if (activeIndex < 0)
            {
                activeIndex += gravityPlanets.Count;
            }

            activeIndex = activeIndex % gravityPlanets.Count;
            Debug.Log("New Index: " + activeIndex);
            SetActivePlanetCam(gravityPlanets.ElementAt(activeIndex));
        }
    }
    public void SetActivePlanetCam(PlanetGravity activePlanet)
    {
        gravityPlanets.ForEach(x => x.vCamera.Priority = 10);
        activePlanet.vCamera.Priority = 11;
        OnActivePlanetChanged.Invoke(activePlanet);
    }
}
