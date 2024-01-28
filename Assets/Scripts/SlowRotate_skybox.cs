using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowRotate_skybox : MonoBehaviour
{
    public Material skyboxMat;
    public float rotationSpeed = 0.02f;
    //private float curr_rotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*var rotStep = rotationSpeed * Time.deltaTime;
        curr_rotation += rotStep;
        if (curr_rotation > 1)
        {
            curr_rotation = 0;
        }
        if (curr_rotation <0)
        {
            curr_rotation = 1;
        }*/
        skyboxMat.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}
