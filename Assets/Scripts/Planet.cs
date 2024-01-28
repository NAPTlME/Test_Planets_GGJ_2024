using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Planet : MonoBehaviour
{
    public float radius; // could maybe get radius in world units from collider?
    public float mass; // should probably change this to mass so it can be used for both small objects as well as celestial bodies
    public Rigidbody rbody;
    public Vector3 previousVelocity;
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        previousVelocity = rbody.velocity;
    }
}
