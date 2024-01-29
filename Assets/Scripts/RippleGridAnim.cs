using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleGridAnim : MonoBehaviour
{

    public Material gridMat;
    public float minRad = 0.01f;
    public float activeRad = 0.192f;
    public float maxRad = 0.4f;

    public float minRippleTime = 0.9f;
    public float maxRippleTime = 3.8f;

    public float minRippleIntensity = 4.3f;
    public float maxRippleIntensity = 13.69f;

    public float temp = 0f;
    public float decayRate = 0.1f;
    public float stepRate = 0.3f;

    public float rippleSpeed = 1f;
    private float currRippleTime = 0f;

    private bool isCalled = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var rad = activeRad;
        bool overwriteRad = false;
        if (!isCalled)
        {
            temp -= decayRate * Time.deltaTime;
            if (temp <= 0)
            {
                temp = 0;
                rad = minRad;
                currRippleTime = 0f;
            }
        }
        else
        {
            if (temp > 1)
            {
                temp = 1;
            }
            
            isCalled = false;
        }
        currRippleTime += rippleSpeed * Time.deltaTime;
        if (currRippleTime > maxRippleTime)
        {
            currRippleTime = minRippleTime;
            overwriteRad = true;
        }
        if (!overwriteRad && temp > 0)
        {
            rad = Mathf.Lerp(activeRad, maxRad, temp);
        }
        float rippleIntensity = Mathf.Lerp(minRippleIntensity, maxRippleIntensity, temp);


        gridMat.SetFloat("_RippleRadius", rad);
        gridMat.SetFloat("_Ripple_time", currRippleTime);
        gridMat.SetFloat("_RippleIntensity", rippleIntensity);
    }

    public void increaseTemp()
    {
        temp += stepRate;
        isCalled = true;
    } 
}
