using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launch_Arrow : MonoBehaviour
{
    public Material ArrowMat; // just overwriting the main instance
    public float MinRatio = 0.6f;
    public float MaxRatio = 4f;
    public float MinStretch = 0.6f;
    public float MaxStretch = 4f;
    public float debug_stretch_speed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // temp
        // stretch to max ratio
        var maxY = MaxRatio * transform.localScale.x;
        var addY = debug_stretch_speed * Time.deltaTime;
        if (transform.localScale.y + addY < maxY)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + addY, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x * MinRatio, transform.localScale.z);
        }

            // get x/z ratio
            var ratio = transform.localScale.y / transform.localScale.x;

        

        // convert to stretch
        var stretch = (ratio - MinRatio) / (MaxRatio - MinRatio) * (MaxStretch - MinStretch) + MinStretch;

        ArrowMat.SetFloat("_squish_scale", stretch);

    }
}
