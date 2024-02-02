using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean_warp : MonoBehaviour
{
    Animator animator;
    public Transform planet_other;

    public float MinDistanceForDeform = 7f;
    public float MinDistanceAllowed = 4f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 deform = Vector3.zero;
        float other_planet_dist = Vector3.Distance(transform.position, planet_other.position);
        //Debug.Log(other_planet_dist);
        if (other_planet_dist <= MinDistanceForDeform)
        {
            // get direction
            var dir_norm = (planet_other.position - transform.position).normalized;
            //Debug.Log("dir_norm: " + dir_norm);
            // get magnitude
            float max_diff = MinDistanceForDeform - MinDistanceAllowed;
            var magnitude = Mathf.Clamp01((MinDistanceForDeform - other_planet_dist) / max_diff);
            var dir = dir_norm * magnitude;
            // calculate blend (y vs xz)
            var blend = 1 - Mathf.Abs(dir_norm.y) / (Mathf.Abs(dir_norm.x) + Mathf.Abs(dir_norm.z) + Mathf.Abs(dir_norm.y));
            //Debug.Log("blend: " + blend);
            deform = dir;
            animator.SetFloat("Blend", blend);
        }
        animator.SetFloat("x", deform.x);
        animator.SetFloat("y", deform.y);
        animator.SetFloat("z", deform.z);
        
    }
}
