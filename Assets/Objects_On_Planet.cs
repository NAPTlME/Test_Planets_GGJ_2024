using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(Rigidbody))]
public class Objects_On_Planet : MonoBehaviour
{
    public Transform HomePlanet; // change to Planet
    public Transform OtherPlanet; // to remove and use the list instead
    public List<Planet> OtherPlanets; // potentially convert to a dictionary instead in which the key is the Planet and the value is the distance to that planet (surface) in the last update (for calculating the impulse)
    public float OtherPlanet_rad = 2f;
    public float planet_rad = 1.75f;
    public float MinDistToFeelGravity = 7f;

    public float distToOtherPlanet = 0f;

    public float distAccelCoef = 1f;
    public float centerOfMassGizmoRad = 0.04f;

    Rigidbody rbody;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        transform.SetParent(HomePlanet);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(HomePlanet.position, transform.position) - planet_rad > Vector3.Distance(OtherPlanet.position, transform.position) - OtherPlanet_rad)
        {
            var newOtherPlanet = HomePlanet;
            HomePlanet = OtherPlanet;
            OtherPlanet = newOtherPlanet;
            distToOtherPlanet = 0f;
            transform.SetParent(HomePlanet);
        }
        var directionToPlanet = (HomePlanet.position - transform.position).normalized;
        var gravityForceFromPlanet = directionToPlanet * 9f;
        var totalGravityForce = gravityForceFromPlanet;

        var ToOtherPlanet = (OtherPlanet.position - transform.position);

        var planetDistances = OtherPlanets.Select(planet => (planet, Vector3.Distance(planet.transform.position, transform.position) - planet.radius)).ToList();
        // define home planet based on closest?
        //Planet = planetDistances.OrderBy(sel => sel.Item2).Select(sel => sel.planet);
        planetDistances.ForEach(s =>
        {
            // gravity calc here,
            // add to totalGravityForce
        });
        if (ToOtherPlanet.magnitude < MinDistToFeelGravity)
        {
            var directionToOtherPlanet = ToOtherPlanet.normalized;
            var newDist = ToOtherPlanet.magnitude;
            var accelCoef = Mathf.Max(distToOtherPlanet - newDist, 0) * distAccelCoef;
            var gravityForceMag = (accelCoef + 1) / Mathf.Pow(newDist, 2f) * Time.fixedDeltaTime;
            
            if (accelCoef > 0.01f) //extra impulse to get interesting behavior 
            {
                rbody.AddForce(directionToOtherPlanet * accelCoef, ForceMode.Impulse);
            }

            var gravityForceFromOtherPlanet = directionToOtherPlanet * 9f * gravityForceMag;

            totalGravityForce += gravityForceFromOtherPlanet;
            distToOtherPlanet = newDist;
        }
        rbody.AddForce(totalGravityForce, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "planet")
        {
            Planet otherPlanet = other.GetComponent<Planet>();
            OtherPlanets.Add(otherPlanet);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "planet")
        {
            Planet otherPlanet = other.GetComponent<Planet>();
            OtherPlanets.Remove(otherPlanet);
        }
    }
    private void OnDrawGizmos()
    {
        if (rbody != null)
        {
            var CoM = rbody.worldCenterOfMass;

            Gizmos.DrawSphere(CoM, centerOfMassGizmoRad);
        }
    }
}
