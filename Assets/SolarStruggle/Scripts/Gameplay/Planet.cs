using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
[ExecuteInEditMode]
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


    public float radius { get; private set; }
    public float mass; // should probably change this to mass so it can be used for both small objects as well as celestial bodies
    public Rigidbody rbody;
    public Vector3 previousVelocity;
    public string planetName;
    public TrailRenderer trailRenderer;
    // Start is called before the first frame update

    private bool destroyed = false;
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        for (var i = 0; i < System.Enum.GetNames(typeof(Planet_Type)).Length; i++)
        {
            //var type = planetTypes[i];
            //StatsManager.getInstance().PlanetTypePrefabToEnum[type] = PlanetTypePrefabToEnumHackArray[i];
        }
        var rand = new System.Random();
        planetName = PLANET_NAMES[rand.Next(PLANET_NAMES.Count)];
        if (tag == "Sun")
        {
            planetName = "Solaris";
        }
        planetName += " " + rand.Next(1000);
    }

    // Updates the components when variables are changed
    void OnEnable()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        previousVelocity = rbody.velocity;
    }

    void Update()
    {

    }

    public void SetTrailRendererEnabled(bool x)
    {
        trailRenderer.gameObject.SetActive(x);
    }
}
