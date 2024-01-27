using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Planet))]
public class PlanetGravity : MonoBehaviour
{
    public Rigidbody rigidBody;
    public Planet planet;

    private void Start()
    {
        GravityManager.getInstance().RegisterPlanet(this);
    }
}
