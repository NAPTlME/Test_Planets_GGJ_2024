using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net;
using System;
using System.Diagnostics.Tracing;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Objects_On_Planet : MonoBehaviour
{
    public Planet homePlanet;
    public List<Planet> planets;
    // distances are used to calculate prev distance vs new distance in update
    [SerializeField]
    public Dictionary<string, float> planetDistances;

    public float DIST_ACCEL_COEF = 1f;
    public const float CENTER_OF_MASS_GIZMO_RAD = 0.04f;
    public const float BASE_GRAVITY_COEF = 9f;
    public const float MaxSolarSystemDistance = 300f;

    public float ArbitraryVelocityDeltaThreshold = 50f;
    public float VelocityDeltaCatchupRate = 0.1f;

    Rigidbody rbody;
    AudioSource audioSource;
    public float pitchRange = 0.1f;
    private Collider col;
    private Vector3 lastLocalPos;
    public Transform Center;
    public bool CanBeKilled = false;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<Collider>();
        if (homePlanet != null)
        {
            transform.SetParent(homePlanet.localPlanetObj.transform);
        }
        planetDistances = new Dictionary<String, float>();
    }

    private void Update()
    {
        /*if (!col.enabled)
        {
            transform.localPosition = lastLocalPos;
        } else
        {
            lastLocalPos = transform.localPosition;
        }*/
        
        if (homePlanet != null && homePlanet.destroyed)
        {
            // clear home planet
            homePlanet = null;
            transform.SetParent(null, true);
        }
        if (transform.position.magnitude > MaxSolarSystemDistance)
        {
            Destroy(this.gameObject);
        }
    }

    // FixedUpdate runs on a predetermined tick rate (50hz by default)
    void FixedUpdate()
    {
        // remove planets that are destroyed
        planets = planets.Where(planet => planet != null && !planet.destroyed).Distinct().ToList();
        var newPlanetDistances = planets.Select(planet => (planet.planetName, Vector3.Distance(planet.localPlanetObj.transform.position, Center.position) - planet.radius))
            .ToList()
            .ToDictionary(x => x.Item1, x => x.Item2);

        // set minimum distance planet as home planet
        Planet minPlanet = null;
        float minDistance = -1f;
        foreach(var planetDistance in newPlanetDistances)
        {
            if (minPlanet == null || planetDistance.Value < minDistance) {
                minPlanet = planets.Where(wh => wh.planetName == planetDistance.Key).First();
                minDistance = planetDistance.Value;
            }
        };
        if (newPlanetDistances.Count > 0) {
            homePlanet = minPlanet;
            transform.SetParent(homePlanet.localPlanetObj.transform, true);
        } else {
            // case for no planets nearby
            homePlanet = null;
            this.planetDistances = newPlanetDistances;
            return;
        }

        // force from home planet
        var directionToPlanet = (homePlanet.localPlanetObj.transform.position - Center.position).normalized;
        var gravityForceFromPlanet = directionToPlanet * BASE_GRAVITY_COEF * homePlanet.radius;
        var totalGravityForce = gravityForceFromPlanet;
        Debug.Log("Force from home planet: " + gravityForceFromPlanet.magnitude);
        // sum up forces from non home planets
        planets.ForEach(planet =>
        {
            /*if (GameObject.ReferenceEquals(planet, homePlanet)) {
                return;
            }*/
            if (planet.planetName == homePlanet.planetName)
            {
                return;
            }
            var ToOtherPlanet = (planet.localPlanetObj.transform.position - Center.position);
            var directionToOtherPlanet = ToOtherPlanet.normalized;
            var newDist = ToOtherPlanet.magnitude - planet.radius;
            Debug.Log(planet.planetName + " dist: " + newDist);
            var accelCoef = Mathf.Max(this.planetDistances.GetValueOrDefault(planet.planetName, 0) - newDist, 0) * DIST_ACCEL_COEF;
            var gravityForceMag = (accelCoef + 1) / Mathf.Pow(newDist, 2f);

            if (accelCoef > 0.01f) //extra impulse to get interesting behavior 
            {
                //rbody.AddForce(directionToOtherPlanet * accelCoef, ForceMode.Impulse);
            }

            var gravityForceFromOtherPlanet = directionToOtherPlanet * (BASE_GRAVITY_COEF * planet.radius) * gravityForceMag; // base gravity coef by radius for approximating the effect of scale
            Debug.DrawLine(Center.position, planet.localPlanetObj.transform.position);
            Debug.Log("Force applied by " + planet.planetName + ": " + gravityForceFromOtherPlanet.magnitude);
            totalGravityForce += gravityForceFromOtherPlanet;
            this.planetDistances = newPlanetDistances;
        });
        rbody.AddForce(totalGravityForce, ForceMode.Acceleration);
    }

    public void SetCanBeKilled(float t)
    {
        StartCoroutine(SetCanBeKilledInTSeconds(t));
    }
    IEnumerator SetCanBeKilledInTSeconds(float t)
    {
        var endTime = Time.time + t;
        while (Time.time < endTime)
        {
            yield return true;
        }
        CanBeKilled = true;
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (obj.CompareTag("LocalPlanet"))
        {
            var planet = obj.GetComponentInParent<Planet>();
            planets.Add(planet);
        }
    }

    private void OnTriggerExit(Collider obj)
    {
        if (obj.CompareTag("LocalPlanet"))
        {
            planets.Remove(obj.GetComponentInParent<Planet>());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (CanBeKilled && collision.gameObject.CompareTag("Sun"))
        {
            audioSource.pitch = 1 + (UnityEngine.Random.value - 0.5f) * pitchRange;
            audioSource.Play();
            var mesh = GetComponent<MeshRenderer>();
            if (mesh != null)
            {
                mesh.enabled = false;
            }
            Destroy(gameObject, audioSource.clip.length);
        }
    }
    private void OnDrawGizmos()
    {
        if (rbody != null)
        {
            var CoM = rbody.worldCenterOfMass;

            Gizmos.DrawSphere(CoM, CENTER_OF_MASS_GIZMO_RAD);

        }
        if (homePlanet != null && homePlanet.localPlanetObj != null)
        {
            Gizmos.DrawSphere(homePlanet.localPlanetObj.transform.position, 0.5f);
        }
    }
}
