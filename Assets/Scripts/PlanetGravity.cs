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
            var vector = this.gameObject.transform.position - sunPosition;
            var distanceFromSun = vector.magnitude;
            distanceToSun = distanceFromSun;
            if (distanceFromSun > GravityManager.getInstance().MaxDistanceBeforeLost)
            {
                destroyed = true;
                var planet = this.GetComponentInParent<Planet>();
                StatsManager.getInstance().LostPlanet(
                    planet.planetType);
                Destroy(planet.gameObject, 3); // Disappear in 3 secs
            }
            else if (distanceFromSun > GravityManager.getInstance().MaxDistanceBeforeBending)
            {
                // var vectorPerpendicularToTrajectory = Vector3.Cross(vector, Vector3.back);
                var vectorPerpendicularToTrajectory = new Vector3(vector.z, 0, vector.x * -1);
                rigidBody.AddForce(
                    GravityManager.getInstance().BendingForce * (vector / distanceFromSun * -1
                    / GravityManager.getInstance().PullBackToPerpendicularRatio +
                    vectorPerpendicularToTrajectory) * rigidBody.mass
                );
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("This: " + this.gameObject.name + " hit: " + collision.gameObject.name);
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
    }
}
