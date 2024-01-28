using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetAudio : MonoBehaviour
{
    private AudioSource audioSource;

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
        if (collision.gameObject == GravityManager.getInstance().gameObject)
        {
            Debug.Log("Crashing into the sun, playing SFX");
            audioSource.Play();
            Destroy(this.gameObject, audioSource.clip.length);
        }
    }
}
