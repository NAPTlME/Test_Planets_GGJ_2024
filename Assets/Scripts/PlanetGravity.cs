using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Planet))]
public class PlanetGravity : MonoBehaviour
{
    public Rigidbody rigidBody;
    public Planet planet;

    private void OnEnable()
    {
        GravityManager.getInstance().RegisterPlanet(this);
    }
    private void OnDisable()
    {
        GravityManager.getInstance().ForgetPlanet(this);
    }
}
