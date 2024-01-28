using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetGravity : MonoBehaviour
{
    public Rigidbody rigidBody;

    private void OnEnable()
    {
        Debug.Log("GravityPlanet: OnEnable");
        GravityManager.getInstance().RegisterPlanet(this);
    }
    private void OnDisable()
    {
        GravityManager.getInstance().ForgetPlanet(this);
    }

}
