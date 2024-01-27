using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public Rigidbody rigidBody;
    public int Force = 100;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("space"))
        {
            Debug.Log("Adding force along Z");
            rigidBody.AddForce(Vector3.back * Force);
        }
    }
}
