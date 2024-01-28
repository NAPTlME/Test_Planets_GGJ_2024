using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitRig : MonoBehaviour
{
    public bool isFocused;

    public Transform target;

    public Transform defaultTarget;

    [SerializeField]
    [Range(1,100)]
    private float followSpeed = 40f;

    [Range(1,20)]
    public float mouseSensitivity = 10f;

    [Range(1,10)]
    public float maximumMouseSpeed = 5f;

    public float zoomSpeed = 1f;

    public float minDistance = 8f;

    public float maxDistance = 30f;

    private float currentDistance;



    // Start is called before the first frame update
    void Start()
    {
        target = defaultTarget;

        currentDistance = maxDistance;
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

        if (isFocused)
        {
            Vector3 direction = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

            direction.x = Mathf.Clamp(direction.x, -maximumMouseSpeed, maximumMouseSpeed);
            direction.y = Mathf.Clamp(direction.y, -maximumMouseSpeed, maximumMouseSpeed);

            transform.Rotate(new Vector3(1, 0, 0), direction.y * mouseSensitivity);
            transform.Rotate(new Vector3(0, 1, 0), -direction.x * mouseSensitivity);

            Vector3 angles = transform.eulerAngles;

            // Normalize angles from -180 to 180
            if (angles.x >= 180) angles.x -= 360;

            // Clamp range from -90 to 90
            angles.x = Mathf.Clamp(angles.x, -45, 45);

            // Normalize angles from 0 to 360
            if (angles.x < 0) angles.x += 360;

            angles.z = 0;

            transform.eulerAngles = angles;

            currentDistance -= Input.mouseScrollDelta.y * zoomSpeed;

            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

            for(int i=0; i<transform.childCount; i++)
            {
                Transform child = transform.GetChild(i).transform;

                child.localPosition = new Vector3(0, 0, -currentDistance);
            }

        }
    }
}
