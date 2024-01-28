using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    private AudioSource audioSource;
    public static MainMenuMusic singleton; 
    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }

        singleton = this;
        Debug.Log("Awake");
        DontDestroyOnLoad(transform.gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        if (audioSource.isPlaying) return;
        audioSource.Play();
    }

    public void StopMusic()
    {
        Debug.Log("Stop");
        audioSource.Stop();
    }
}
