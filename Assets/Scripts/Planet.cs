using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlanetTypeToStatsTypeTuple
{
    public PlanetType planetType;
    public StatsPlanetType statsPlanetType;
}

[RequireComponent(typeof(Rigidbody))]
[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    public PlanetType planetType;
    public List<PlanetType> planetTypes;
    public List<StatsPlanetType> PlanetTypePrefabToEnumHackArray;


    public float radius { get; private set; }
    public float mass; // should probably change this to mass so it can be used for both small objects as well as celestial bodies
    public Rigidbody rbody;
    public Vector3 previousVelocity;
    // Start is called before the first frame update

    private bool destroyed = false;
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        ApplyPlanetType();
        for (var i = 0; i < planetTypes.Count; i++)
        {
            var type = planetTypes[i];
            StatsManager.getInstance().PlanetTypePrefabToEnum[type] = PlanetTypePrefabToEnumHackArray[i];
        }
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

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!destroyed && collision.gameObject.tag == "Sun")
        {
            Debug.Log("Crashing into the sun, BURN!");
            var exp = GetComponent<ParticleSystem>();
            exp.Play();
            GetComponent<MeshRenderer>().enabled = false;
            StatsManager.getInstance().KillResidents(this.planetType);
            // Note: we could use `this.enabled = false` instead but I don't
            // trust it to happen soon enough before the next run/couple of runs of
            // the physics, the boolean should be trusthworthy
            destroyed = true;
        }
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
