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
        Debug.Log("GravityPlanet: OnEnable");
        GravityManager.getInstance().RegisterPlanet(this);
    }
    private void OnDisable()
    {
        GravityManager.getInstance().ForgetPlanet(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == GravityManager.getInstance().gameObject)
        {
            Debug.Log("Crashing into the sun, BURN!");
            Destroy(this.gameObject);
        }
    }
}
