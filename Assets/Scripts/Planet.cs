using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    public PlanetType planetType;
    public List<PlanetType> planetTypes;
    public float radius { get; private set; }
    public float mass; // should probably change this to mass so it can be used for both small objects as well as celestial bodies
    public Rigidbody rbody;
    public Vector3 previousVelocity;
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        ApplyPlanetType();
    }

    // Updates the components when variables are changed
    void OnEnable()
    {
        ApplyPlanetType();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        previousVelocity = rbody.velocity;
    }

    private void ApplyPlanetType()
    {
        if (planetType is null) return;

        // Set the material of the mesh
        GetComponent<MeshRenderer>().material = planetType.tileMaterial;

        // Set the mass
        GetComponent<Rigidbody>().mass = planetType.mass;

        // Set the scale
        float scale = planetType.scale;
        transform.localScale = new Vector3(scale, scale, scale);
        radius = scale;
    }
}
