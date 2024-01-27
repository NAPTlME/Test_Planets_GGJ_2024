using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public delegate void OnLoadFinished(bool faded);
    public static event OnLoadFinished onLoadFinished;

    public Image[] loadingScreenImages;
    public TMP_Text[] loadingScreenTexts;
    float currentFillPerc = 1.0f;
    int fadeDir = 0;
    Coroutine currentFadeRoutine;

    public void FadeIn(float newFadeTime)
    {
        fadeDir = 1;
        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
        currentFadeRoutine = StartCoroutine(ToggleFade(newFadeTime));
    }

    public void FadeOut(float waitTime, float newFadeTime)
    {
        fadeDir = -1;
        StartCoroutine(FadeAfterTime(waitTime, newFadeTime));
    }

    IEnumerator FadeAfterTime(float timeToWait, float newFadeTime)
    {
        yield return new WaitForSeconds(timeToWait);
        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
        currentFadeRoutine = StartCoroutine(ToggleFade(newFadeTime));
    }

    IEnumerator ToggleFade(float newFadeTime)
    {
        for (float i = 0; i < newFadeTime; i += Time.deltaTime)
        {
            currentFillPerc += fadeDir * (Time.deltaTime / newFadeTime);
            currentFillPerc = Mathf.Clamp01(currentFillPerc);

            for (int j = 0; j < loadingScreenImages.Length; j++)
            {
                loadingScreenImages[j].color = new Color(loadingScreenImages[j].color.r, loadingScreenImages[j].color.g, loadingScreenImages[j].color.b, currentFillPerc);
                loadingScreenTexts[j].color = new Color(loadingScreenTexts[j].color.r, loadingScreenTexts[j].color.g, loadingScreenTexts[j].color.b, currentFillPerc);
            }            
            yield return null;
        }

        if(fadeDir < 0)
        {
            currentFillPerc = 0.0f;
            onLoadFinished?.Invoke(true);
        }
        else
        {
            currentFillPerc = 1.0f;
            onLoadFinished?.Invoke(false);
        }

        for (int j = 0; j < loadingScreenImages.Length; j++)
        {
            loadingScreenImages[j].color = new Color(loadingScreenImages[j].color.r, loadingScreenImages[j].color.g, loadingScreenImages[j].color.b, currentFillPerc);
            loadingScreenTexts[j].color = new Color(loadingScreenTexts[j].color.r, loadingScreenTexts[j].color.g, loadingScreenTexts[j].color.b, currentFillPerc);
        }
    }
}
