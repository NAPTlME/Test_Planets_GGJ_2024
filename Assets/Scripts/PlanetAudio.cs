using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private bool destroyed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!destroyed && collision.gameObject.tag == "Sun")
        {
            // Debug.Log("Crashing into the sun, playing SFX");
            audioSource.Play();
            // Note: we could use `this.enabled = false` instead but I don't
            // trust it to happen soon enough before the next run/couple of runs of
            // the physics, the boolean should be trusthworthy
            destroyed = true;
            Destroy(this.gameObject, audioSource.clip.length);
        }
    }
}
