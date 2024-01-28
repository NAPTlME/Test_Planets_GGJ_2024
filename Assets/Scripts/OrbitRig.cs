using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitRig : MonoBehaviour
{
    public Transform target;

    public Transform defaultTarget;

    [SerializeField]
    [Range(1,100)]
    private float followSpeed = 20f;

    private Vector3 previousMousePosition;


    // Start is called before the first frame update
    void Start()
    {
        target = defaultTarget;
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            target = defaultTarget;
        }
        {
            // Lerp between current position and target position for smooth motion to targeted planet
            Vector3 deltaPosition = target.position - transform.position;
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
        }

        // Get player mouse input and rotate the rig accordingly\
        if(Input.GetMouseButtonDown(1))
        {
            previousMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(1))
        {
            Vector3 direction = previousMousePosition - Camera.main.ScreenToViewportPoint(Input.mousePosition);

            transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180);

            previousMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }
        
    }
}
