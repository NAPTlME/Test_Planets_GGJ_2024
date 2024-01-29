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
        //Debug.Log("GravityPlanet: OnEnable");
        GravityManager.getInstance().RegisterPlanet(this);
    }
    private void OnDisable()
    {
        GravityManager.getInstance().ForgetPlanet(this);
    }
    private void FixedUpdate()
    {
        var sun = GravityManager.getInstance().gameObject;
        if (!destroyed && this.gameObject.tag != "Sun")
        {
            var sunPosition = sun.transform.position;
            var vector = this.gameObject.transform.position - sunPosition;
            var distanceFromSun = vector.magnitude;
            distanceToSun = distanceFromSun;
            if (distanceFromSun > GravityManager.MaxDistanceBeforeLost)
            {
                destroyed = true;
                StatsManager.getInstance().LostPlanet(
                    this.gameObject.GetComponent<Planet>().planetType);
                Destroy(this.gameObject, 3); // Disappear in 3 secs
            }
            else

            if (distanceFromSun > GravityManager.MaxDistanceBeforeBending)
            {
                //Debug.Log(Time.frameCount + "  " + distanceFromSun);
                //Debug.Log("Adding perpendicular force!");
                // var vectorPerpendicularToTrajectory = Vector3.Cross(vector, Vector3.back);
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
