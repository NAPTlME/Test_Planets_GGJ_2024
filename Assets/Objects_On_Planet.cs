using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Planet : MonoBehaviour
{
    public float radius;
    public float gravitational_coef;
}

[RequireComponent(typeof(Rigidbody))]
public class Objects_On_Planet : MonoBehaviour
{
    public Transform Planet; // change to Planets
    public Transform OtherPlanet; // to remove and use the list instead
    public List<Planet> OtherPlanets;
    public float OtherPlanet_rad = 2f;
    public float planet_rad = 1.75f;
    public float MinDistToFeelGravity = 7f;

    public float distToOtherPlanet = 0f;

    public float distAccelCoef = 1f;

    Rigidbody rbody;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        transform.SetParent(Planet);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(Planet.position, transform.position) - planet_rad > Vector3.Distance(OtherPlanet.position, transform.position) - OtherPlanet_rad)
        {
            var newOtherPlanet = Planet;
            Planet = OtherPlanet;
            OtherPlanet = newOtherPlanet;
            distToOtherPlanet = 0f;
            transform.SetParent(Planet);
        }
        var directionToPlanet = (Planet.position - transform.position).normalized;
        var gravityForceFromPlanet = directionToPlanet * 9f;
        var totalGravityForce = gravityForceFromPlanet;

        var ToOtherPlanet = (OtherPlanet.position - transform.position);

        var planetDistances = OtherPlanets.Select(sel => (sel, Vector3.Distance(sel.transform.position, transform.position) - sel.radius)).ToList();
        // define home planet based on closest?
        //Planet = planetDistances.OrderBy(sel => sel.Item2).Select(sel => sel.sel);
        planetDistances.ForEach(s =>
        {
            // gravity calc here,
            // add to totalGravityForce
        });
        if (ToOtherPlanet.magnitude < MinDistToFeelGravity)
        {
            var directionToOtherPlanet = ToOtherPlanet.normalized;
            var newDist = ToOtherPlanet.magnitude;
            var gravityForceMag = (Mathf.Max(distToOtherPlanet - newDist, 0) * distAccelCoef + 1) / Mathf.Pow(newDist, 2f) * Time.fixedDeltaTime;
            Debug.Log("---------");
            Debug.Log("distAccel: " + Mathf.Max(distToOtherPlanet - newDist, 0) * distAccelCoef);
            Debug.Log("Dist: " + distToOtherPlanet);
            Debug.Log("gravityForceMag: " + gravityForceMag);
            var distImpulse = Mathf.Max(distToOtherPlanet - newDist, 0) * distAccelCoef;
            if (distImpulse > 0.01f)
            {
                rbody.AddForce(directionToOtherPlanet * distImpulse, ForceMode.Impulse);
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
}
