using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public static bool isLoading = false;
    public static bool isStory = true;
    public AudioSource mySource;
    public AudioClip[] uiSounds;
    LoadingScreen loadingScreen;

    public void Awake()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }

        singleton = this;

        DontDestroyOnLoad(gameObject);
        loadingScreen = GetComponent<LoadingScreen>();
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg1 != LoadSceneMode.Additive)
        {
            PlaySceneUnLoad();
        }
    }

    public void PlaySceneLoad(string sceneName)
    {
        StartCoroutine(LoadSceneAfterFade(sceneName));
        isLoading = true;
    }

    IEnumerator LoadSceneAfterFade(string sceneName)
    {
        singleton.loadingScreen.FadeIn(.5f);
        yield return new WaitForSeconds(1.0f);
        if (DoesSceneExist(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            yield break;
        }
        else
        {
            Debug.Log(sceneName +" Does Not Exist. Loading TitleScreen");
            SceneManager.LoadScene("TitleScreen");
        }
    }

    bool DoesSceneExist(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            var lastSlash = scenePath.LastIndexOf("/");
            var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

            if (string.Compare(name, sceneName, true) == 0)
                return true;
        }

        return false;
    }

    public void PlaySceneUnLoad()
    {
        singleton.loadingScreen.FadeOut(.5f, 1.0f);
        isLoading = false;
    }

    public static void PlayButtonSound(int soundID)
    {
        singleton.mySource.PlayOneShot(singleton.uiSounds[soundID]);
    }
}