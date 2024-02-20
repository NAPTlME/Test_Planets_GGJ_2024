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
    private TrailRendererAnimData defaultTrailRendererFields;
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

        defaultTrailRendererFields = new TrailRendererAnimData(trailRenderer);
    }

    void LateUpdate()
    {
        //if (!destroyed)
        //{
            localPlanetObj.transform.SetPositionAndRotation(orbitalPlanetObj.transform.position, orbitalPlanetObj.transform.rotation);
        //}
        //transform.SetPositionAndRotation(orbitalPlanetObj.transform.position, orbitalPlanetObj.transform.rotation);
    }

    public void InitialTrailRenderer(float _startWidth, Color _startColor, float _duration)
    {
        trailRenderer.startWidth = _startWidth;
        trailRenderer.endWidth = _startWidth;
        trailRenderer.startColor = _startColor;
        StartCoroutine(LerpInitialTrailToDefault(defaultTrailRendererFields.startWidth, _duration));
    }

    IEnumerator LerpInitialTrailToDefault(float _targetWidth, float _duration)
    {
        var startTime = Time.time;
        var endTime = startTime + _duration;
        var currWidth = trailRenderer.startWidth;
        var startWidth = trailRenderer.startWidth;
        var width_step = (_targetWidth - startWidth) / _duration;
        while (Time.time < endTime)
        {
            currWidth += width_step * Time.deltaTime;
            trailRenderer.startWidth = currWidth;
            yield return null;
        }
        trailRenderer.startWidth = _targetWidth;
    }

    
    public void SetTrailRendererEnabled(bool x)
    {
        trailRenderer.gameObject.SetActive(x);
    }
}

public class TrailRendererAnimData
{
    public float startWidth;
    public float endWidth;
    public Color startColor;

    public TrailRendererAnimData(float _startWidth, float _endWidth, Color _startColor)
    {
        startWidth = _startWidth;
        endWidth = _endWidth;
        startColor = _startColor;
    }

    public TrailRendererAnimData(TrailRenderer refTr)
    {
        startWidth = refTr.startWidth;
        endWidth = refTr.endWidth;
        startColor = refTr.startColor;
    }
}
