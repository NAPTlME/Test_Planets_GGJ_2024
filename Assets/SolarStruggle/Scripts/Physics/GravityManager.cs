using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GravityManager : MonoBehaviour
{

    public static GravityManager Instance { get; private set; }
    public const double MaxDistanceBeforeBending = 90f; // checked when it seemed "too far"
    public const float BendingForce = 0.0018f;

    public const float MaxDistanceBeforeLost = 190f;

    public const float PullBackToPerpendicularRatio = 0.0001f;
    // Not a real constant so we can use Double.Parse()
    static private double GRAVITY = 1;

    // This does not need to be public but it makes inspecting it in the inspector for debugging
    // quite convenient
    public List<PlanetGravity> gravityPlanets = new List<PlanetGravity>();

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
        if (Instance is not null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
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