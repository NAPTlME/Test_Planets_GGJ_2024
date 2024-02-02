using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DodecPlanetBuild))]
public class Test_MakePlanets : MonoBehaviour
{
    DodecPlanetBuild planetBuild;
    public Planet lastPlanet;
    // Start is called before the first frame update
    void Start()
    {
        planetBuild = GetComponent<DodecPlanetBuild>();
        var planet = planetBuild.Build();
        lastPlanet = planet;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(lastPlanet);
            var planet = planetBuild.Build();
            lastPlanet = planet;
        }
    }
}
