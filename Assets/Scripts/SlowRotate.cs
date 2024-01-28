using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SlowRotate : MonoBehaviour
{
    public Vector3 RotateAxis = Vector3.up + Vector3.left * 2;
    public float RotateSpeed = 2f;
    private Rigidbody rbody;
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var rotation = Quaternion.AngleAxis(RotateSpeed * Time.fixedDeltaTime, RotateAxis);
        rbody.MoveRotation(transform.rotation * rotation);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, RotateAxis);
    }
}
