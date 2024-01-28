using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetGravity : MonoBehaviour
{
    public Rigidbody rigidBody;

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
        if (this.gameObject.tag != "Sun")
        {
            var sunPosition = sun.transform.position;
            var vector = this.gameObject.transform.position - sunPosition;
            var distanceFromSun = vector.magnitude;
            if (distanceFromSun > GravityManager.getInstance().MaxDistanceBeforeBending && distanceFromSun < GravityManager.getInstance().MaxDistanceBeforeBending * 10)
            {
                //Debug.Log(Time.frameCount + "  " + distanceFromSun);
                //Debug.Log("Adding perpendicular force!");
                // var vectorPerpendicularToTrajectory = Vector3.Cross(vector, Vector3.back);
                var vectorPerpendicularToTrajectory = new Vector3(vector.z, 0, vector.x * -1);
                rigidBody.AddForce(
                    GravityManager.getInstance().BendingForce * (vector / distanceFromSun * -1
                    / GravityManager.getInstance().PullBackToPerpendicularRatio +
                    vectorPerpendicularToTrajectory)
                );
            }
        }
    }
}
