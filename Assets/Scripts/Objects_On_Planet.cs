using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net;
using System;
using System.Diagnostics.Tracing;

[RequireComponent(typeof(Rigidbody))]
public class Objects_On_Planet : MonoBehaviour
{
    public Planet homePlanet;
    public List<Planet> planets;
    // distances are used to calculate prev distance vs new distance in update
    public Dictionary<Planet, float> planetDistances;

    public const float DIST_ACCEL_COEF = 1f;
    public const float CENTER_OF_MASS_GIZMO_RAD = 0.04f;
    public const float BASE_GRAVITY_COEF = 9f;

    Rigidbody rbody;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("init planet count: " + planets.Count);
        rbody = GetComponent<Rigidbody>();
        planetDistances = new Dictionary<Planet, float>();
        var minDist = -1f;
        foreach(var planet in planets)
        {
            var dist = Vector3.Distance(planet.transform.position, transform.position) - planet.radius;
            planetDistances.Add(planet, dist);
            if (homePlanet == null || dist < minDist)
            {
                minDist = dist;
                homePlanet = planet;
            }
        }
        if (homePlanet != null)
        {
            transform.SetParent(homePlanet.transform);
        }
    }

    // FixedUpdate runs on a predetermined tick rate (50hz by default)
    void FixedUpdate()
    {
        // Debug.Log("planet count: " + planets.Count);
        var newPlanetDistances = planets.Select(planet => (planet, Vector3.Distance(planet.transform.position, transform.position) - planet.radius))
            .ToList()
            .ToDictionary(x => x.Item1, x => x.Item2);

        // set minimum distance planet as home planet
        Planet minPlanet = null;
        float minDistance = -1f;
        foreach(var planetDistance in newPlanetDistances)
        {
            if (minPlanet == null || planetDistance.Value < minDistance) {
                minPlanet = planetDistance.Key;
                minDistance = planetDistance.Value;
            }
        };
        if (newPlanetDistances.Count > 0) {
            homePlanet = minPlanet;
            transform.SetParent(homePlanet.transform);
        } else {
            // case for no planets nearby
            homePlanet = null;
            this.planetDistances = newPlanetDistances;
            return;
        }

        // force from home planet
        var directionToPlanet = (homePlanet.transform.position - transform.position).normalized;
        var gravityForceFromPlanet = directionToPlanet * BASE_GRAVITY_COEF;
        var totalGravityForce = gravityForceFromPlanet;

        // sum up forces from non home planets
        planets.ForEach(planet =>
        {
            if (GameObject.ReferenceEquals(planet, homePlanet)) {
                return;
            }
            var ToOtherPlanet = (planet.transform.position - transform.position);
            var directionToOtherPlanet = ToOtherPlanet.normalized;
            var newDist = ToOtherPlanet.magnitude - planet.radius;
            var accelCoef = Mathf.Max(this.planetDistances.GetValueOrDefault(planet, 0) - newDist, 0) * DIST_ACCEL_COEF;
            var gravityForceMag = (accelCoef + 1) / Mathf.Pow(newDist, 2f);

            if (accelCoef > 0.01f) //extra impulse to get interesting behavior 
            {
                //rbody.AddForce(directionToOtherPlanet * accelCoef, ForceMode.Impulse);
            }

            var gravityForceFromOtherPlanet = directionToOtherPlanet * BASE_GRAVITY_COEF * gravityForceMag;

            totalGravityForce += gravityForceFromOtherPlanet;
            this.planetDistances = newPlanetDistances;
        });
        rbody.AddForce(totalGravityForce, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (obj.tag == "Planet")
        {
            planets.Add(obj.GetComponent<Planet>());
        }
    }

    private void OnTriggerExit(Collider obj)
    {
        if (obj.tag == "Planet")
        {
            planets.Remove(obj.GetComponent<Planet>());
        }
    }
    private void OnDrawGizmos()
    {
        if (rbody != null)
        {
            var CoM = rbody.worldCenterOfMass;

            Gizmos.DrawSphere(CoM, CENTER_OF_MASS_GIZMO_RAD);
        }
    }
}
