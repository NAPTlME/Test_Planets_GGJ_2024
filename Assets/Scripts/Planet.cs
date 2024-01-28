using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    public PlanetType planetType;
    public List<PlanetType> planetTypes;

    public float radius { get; private set; }

    void Start()
    {
        ApplyPlanetType();
    }

    // Updates the components when variables are changed
    void OnEnable()
    {
        ApplyPlanetType();
    }

    void Update()
    {
        
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
