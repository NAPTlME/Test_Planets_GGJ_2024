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
    public TrailRenderer trailRenderer, defaultTrailRenderer;
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

        defaultTrailRenderer = trailRenderer;
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
        StartCoroutine(LerpInitialTrailToDefault(defaultTrailRenderer.startWidth, _duration, _duration));
    }

    IEnumerator LerpInitialTrailToDefault(float _targetWidth, float _duration, float _lerpDuration)
    { 
        yield return new WaitForSeconds(_duration);
    //     float startFill = _experienceBarFill.fillAmount;

    //     // We'll scale our lerp speed so the total duration of the animation is
    //     // proportionate to how much the value has changed.
            float speed = 1.0f / (_lerpDuration * Mathf.Abs(_targetWidth - trailRenderer.startWidth));
     
         for(float t = 0; t < 1f; t += speed * Time.deltaTime) {
            // I've replaced your exponential ease-out Lerp with a quadratic version that
            // completes in a finite number of steps, and is easier to correct for deltaTime.
            // (The exponential one never quite reaches its target)
            float progress = 1f - t;
            progress = 1f - progress * progress;
            trailRenderer.startWidth = Mathf.Lerp(trailRenderer.startWidth, _targetWidth, progress);
        }
        yield return null;
    }

    
    public void SetTrailRendererEnabled(bool x)
    {
        trailRenderer.gameObject.SetActive(x);
    }
}
