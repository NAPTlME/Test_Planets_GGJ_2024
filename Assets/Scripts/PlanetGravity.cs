using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetGravity : MonoBehaviour
{
    public Rigidbody rigidBody;
    public CinemachineFreeLook vCamera;
    public bool destroyed = false;

    public float distanceToSun { get; private set; }

    private void OnEnable()
    {
        vCamera.gameObject.SetActive(true);
        GravityManager.getInstance().RegisterPlanet(this);
    }
    private void OnDisable()
    {
        GravityManager.getInstance().ForgetPlanet(this);
    }
    public void SetCameraPriority(int x)
    {
        vCamera.Priority = x;
    }
    private void FixedUpdate()
    {
        var sun = GravityManager.getInstance().gameObject;
        if (!destroyed && !gameObject.CompareTag("Sun"))
        {
            var sunPosition = sun.transform.position;
            var fromSun = this.gameObject.transform.position - sunPosition;
            var distanceFromSun = fromSun.magnitude;
            distanceToSun = distanceFromSun;
            if (distanceFromSun > GravityManager.MaxDistanceBeforeLost)
            {
                destroyed = true;
                var planet = this.GetComponentInParent<Planet>();
                StatsManager.getInstance().LostPlanet(
                    planet.planetType);
                Destroy(planet.gameObject, 3); // Disappear in 3 secs
            }
            else if (distanceFromSun > GravityManager.MaxDistanceBeforeBending)
            {
                // only apply force if heading further from the sun
                if (Vector3.Dot(rigidBody.velocity, fromSun) > 0)
                {
                    var vectorPerpendicularToTrajectory = Vector3.ProjectOnPlane(-fromSun, rigidBody.velocity);
                    var force = (float)((distanceFromSun - GravityManager.MaxDistanceBeforeBending) /  // blending based on distance. Could also square this for a tighter curve
                        (GravityManager.MaxDistanceBeforeLost - GravityManager.MaxDistanceBeforeBending)) * 
                        GravityManager.MaxBendingForce * rigidBody.mass * 
                        (-fromSun * GravityManager.PullBackToPerpendicularRatio + vectorPerpendicularToTrajectory).normalized;
                    rigidBody.AddForce(force);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("This: " + this.gameObject.name + " hit by: " + collision.gameObject.name);
        if (!destroyed && collision.gameObject.CompareTag("Sun"))
        {
            Debug.Log("Crashing into the sun, BURN!");
            var exp = GetComponent<ParticleSystem>();
            exp.Play();
            var planet = this.transform.GetComponentInParent<Planet>();
            //GetComponentsInChildren<MeshRenderer>().Select(sel => sel.enabled = false);
            planet.planetCollection.SetActive(false);
            PlanetAudio planetAudio = GetComponent<PlanetAudio>();
            planetAudio.audioSource.Play();
            StatsManager.getInstance().KillResidents(planet.planetType);
            // release entities
            planet.localPlanetObj.GetComponentsInChildren<Objects_On_Planet>().ToList().ForEach(x => x.transform.SetParent(null, true));
            planet.localPlanetObj.SetActive(false);
            // Note: we could use `this.enabled = false` instead but I don't
            // trust it to happen soon enough before the next run/couple of runs of
            // the physics, the boolean should be trusthworthy
            destroyed = true;
            GravityManager.getInstance().ForgetPlanet(this);
            Destroy(planet.gameObject, planetAudio.audioSource.clip.length);
        }
        if (gameObject.CompareTag("Sun") && collision.gameObject.CompareTag("Entities"))
        {
            Debug.Log("Start Sizzle");
            Debug.Log("Hit at: " + collision.GetContact(0).point);
            GlobalManager.getInstance().SizzleAt(collision.GetContact(0));
        }
        if (gameObject.CompareTag("Sun"))
        {
            Debug.Log("other tag: " + collision.gameObject.tag);
        }
        if (gameObject.CompareTag("Sun") && collision.gameObject.CompareTag("Planet"))
        {
            // todo get impulse and determine a good range to determine the number of particles to emit
            Debug.Log("Impulse: " + collision.impulse.magnitude);
            GlobalManager.getInstance().BurstAt(collision.GetContact(0), collision.impulse.magnitude);
        }
    }
}
