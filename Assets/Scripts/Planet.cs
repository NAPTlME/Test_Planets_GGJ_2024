using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Planet : MonoBehaviour
{
    public List<string> PLANET_NAMES = new List<string>()
    {
        "Vertigo",
        "Ulia",
        "Pluto",
        "Ceres",
        "Makemake",
        "Haumea",
        "Eris",
        "Arkas",
        "Galileo",
        "Dagon",
        "Wangshu",
        "Veles",
        "Makropulos",
        "Belisama"
    };
    public Planet_Type planetType;
    public GameObject planetCollection;


    public float radius;
    public string planetName;
    public TrailRenderer trailRenderer;
    public GameObject orbitalPlanetObj;
    public GameObject localPlanetObj;
    // Start is called before the first frame update

    public bool destroyed = false;
    void Start()
    {
        var rand = new System.Random();
        planetName = PLANET_NAMES[rand.Next(PLANET_NAMES.Count)];
        if (CompareTag("Sun"))
        {
            planetName = "Solaris";
        }
        planetName += " " + rand.Next(1000);
    }

    void LateUpdate()
    {
        //if (!destroyed)
        //{
            localPlanetObj.transform.SetPositionAndRotation(orbitalPlanetObj.transform.position, orbitalPlanetObj.transform.rotation);
        //}
        //transform.SetPositionAndRotation(orbitalPlanetObj.transform.position, orbitalPlanetObj.transform.rotation);
    }

    
    public void SetTrailRendererEnabled(bool x)
    {
        trailRenderer.gameObject.SetActive(x);
    }
}
