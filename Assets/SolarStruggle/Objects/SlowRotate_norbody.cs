using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SlowRotate_norbody : MonoBehaviour
{
    public Vector3 RotateAxis = Vector3.up + Vector3.left * 2;
    public float RotateSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var rotation = Quaternion.AngleAxis(RotateSpeed * Time.fixedDeltaTime, RotateAxis);
        transform.localRotation = transform.localRotation * rotation;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, RotateAxis);
    }
}
