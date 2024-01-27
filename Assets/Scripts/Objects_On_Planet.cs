using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net;

[RequireComponent(typeof(Rigidbody))]
public class Objects_On_Planet : MonoBehaviour
{
    public Planet homePlanet; // change to Planet
    public List<Planet> planets; // potentially convert to a dictionary instead in which the key is the Planet and the value is the distance to that planet (surface) in the last update (for calculating the impulse)

    public const float DIST_ACCEL_COEF = 1f;
    public const float CENTER_OF_MASS_GIZMO_RAD = 0.04f;
    public const float BASE_GRAVITY_COEF = 9f;

    Rigidbody rbody;

    // Start is called before the first frame update
    void Start()
    {
        planets = new List<Planet>();
        rbody = GetComponent<Rigidbody>();
        transform.SetParent(homePlanet.transform);
    }

    // FixedUpdate runs on a predetermined tick rate (50hz by default)
    void FixedUpdate()
    {
        var newPlanetDistances = planets.Select(planet => (planet, Vector3.Distance(planet.transform.position, transform.position) - planet.radius)).ToList();
        
        // set minimum distance planet as home planet
        var minPlanetDistance = newPlanetDistances[0];
        newPlanetDistances.ForEach(planetDistance =>
        {
            if (planetDistance.Item2 < minPlanetDistance.Item2)
            {
                minPlanetDistance = planetDistance;
            }
        });
        homePlanet = minPlanetDistance.Item1;
        transform.SetParent(homePlanet.transform);

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
            var directionToPlanet = (homePlanet.transform.position - transform.position).normalized;
            var gravityForceFromPlanet = directionToPlanet * BASE_GRAVITY_COEF;
            var totalGravityForce = gravityForceFromPlanet;
            var ToOtherPlanet = (planet.transform.position - transform.position);
        });
        

        var ToOtherPlanet = (OtherPlanet.position - transform.position);

        var planetDistances = OtherPlanets.Select(planet => (planet, Vector3.Distance(planet.transform.position, transform.position) - planet.radius)).ToList();
        // define home planet based on closest?
        //Planet = planetDistances.OrderBy(sel => sel.Item2).Select(sel => sel.planet);
        newPlanetDistances.ForEach(s =>
        {
            // gravity calc here,
            // add to totalGravityForce
        });
        if (ToOtherPlanet.magnitude < MinDistToFeelGravity)
        {
            var directionToOtherPlanet = ToOtherPlanet.normalized;
            var newDist = ToOtherPlanet.magnitude;
            var accelCoef = Mathf.Max(distToOtherPlanet - newDist, 0) * DIST_ACCEL_COEF;
            var gravityForceMag = (accelCoef + 1) / Mathf.Pow(newDist, 2f) * Time.fixedDeltaTime;
            
            if (accelCoef > 0.01f) //extra impulse to get interesting behavior 
            {
                rbody.AddForce(directionToOtherPlanet * accelCoef, ForceMode.Impulse);
            }

            var gravityForceFromOtherPlanet = directionToOtherPlanet * BASE_GRAVITY_COEF * gravityForceMag;

            totalGravityForce += gravityForceFromOtherPlanet;
            distToOtherPlanet = newDist;
        }
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
