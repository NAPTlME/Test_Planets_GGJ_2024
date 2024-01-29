using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public static GravityManager getInstance()
    {
        return instance;
    }

    public void RegisterPlanet(PlanetGravity planetGravity)
    {
        // Debug.Log("registering planet");
        gravityPlanets.Add(planetGravity);
    }

    public void ForgetPlanet(PlanetGravity planetGravity)
    {
        // TODO: This is inefficient, we should look into using sets, but IDK C#'s default hashing requirements
        // and there won't be many planets so this is good enough for now
        // Debug.Log("forgetting planet");
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
}
