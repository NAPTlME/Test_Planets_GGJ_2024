using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetGravity : MonoBehaviour
{
    public Rigidbody rigidBody;
    private bool destroyed = false;

    public float distanceToSun;

    private void OnEnable()
    {
        GravityManager.Instance.RegisterPlanet(this);
    }

    private void OnDisable()
    {
        GravityManager.Instance.ForgetPlanet(this);
    }

    private void FixedUpdate()
    {
        if (!destroyed && this.gameObject.tag != "Sun")
        {
            var vector = this.gameObject.transform.position;
            var distanceFromSun = vector.magnitude;
            distanceToSun = distanceFromSun;
            if (distanceFromSun > GravityManager.MaxDistanceBeforeLost)
            {
                destroyed = true;
                StatsManager.Instance.LostPlanet(
                    this.gameObject.GetComponent<Planet>().planetType);
                Destroy(this.gameObject, 3); // Disappear in 3 secs
            }
            else

            if (distanceFromSun > GravityManager.MaxDistanceBeforeBending)
            {
                var vectorPerpendicularToTrajectory = new Vector3(vector.z, 0, vector.x * -1);
                var force = GravityManager.BendingForce
                * (vector / distanceFromSun * -1
                    / GravityManager.PullBackToPerpendicularRatio +
                    vectorPerpendicularToTrajectory)
                    * rigidBody.mass;
                Debug.Log(force);
                rigidBody.AddForce(force);
            }
        }
    }
}
