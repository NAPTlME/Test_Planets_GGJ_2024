using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launch_Arrow : MonoBehaviour
{
    public GameObject Arrow_Child;
    private Material ArrowMat; // just overwriting the main instance for now
    public float MinStretch = 0.6f;
    public float meters_per_scale = 2f;
    public float y_plane = 0f;
    // Start is called before the first frame update
    void Start()
    {
        ArrowMat = Arrow_Child.GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdatePosition(Vector2 StartPoint_world_2d, Vector2 EndPoint_world_2d)
    {
        // must be flat with the y plane
        var startPoint_world_3d = new Vector3(StartPoint_world_2d.x, y_plane, StartPoint_world_2d.y);
        var endPoint_world_3d = new Vector3(EndPoint_world_2d.x, y_plane, EndPoint_world_2d.y);
        var angleToRotate = Vector3.SignedAngle(Vector3.forward, startPoint_world_3d - endPoint_world_3d, Vector3.up);
        var rotation = Quaternion.AngleAxis(angleToRotate, Vector3.up);
        var center = (startPoint_world_3d + endPoint_world_3d) / 2;


        transform.SetPositionAndRotation(center, rotation);

        // calculate scale
        var z_scale = Vector3.Distance(StartPoint_world_2d, EndPoint_world_2d) / meters_per_scale;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z_scale);

        // set squish
        SetSquish(Mathf.Max(z_scale, 1) * MinStretch);
    }

    public void SetSquish(float stretch)
    {
        // valid values > 0.6f
        ArrowMat.SetFloat("_squish_scale", stretch);
    }

    public void FadeIn(float t_seconds)
    {
        StartCoroutine(Fade(t_seconds, 0f, 1f));
    }
    public void FadeOut(float t_seconds)
    {
        StartCoroutine(Fade(t_seconds, 1f, 0f));
    }
    // assume we are either zero or one when starting this
    IEnumerator Fade(float t_seconds, float start = 0f, float end = 1f)
    {
        var startTime = Time.time;
        var endTime = startTime + t_seconds;
        var currAlpha = start;
        var second_step = (end - start) / t_seconds;
        while (Time.time < endTime)
        {
            currAlpha += second_step * Time.deltaTime;
            SetFade(currAlpha);
            yield return null;
        }
        SetFade(end);
    }
    public void SetFade(float x)
    {
        ArrowMat.SetFloat("_global_alpha", Mathf.Clamp01(x));
    }
}
