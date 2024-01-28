using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyButtonNextScene : MonoBehaviour
{
    // Start is called before the first frame update
    public Scenes nextScene;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene(
                GameManager.GetSceneNameFromEnum(nextScene),
                LoadSceneMode.Single);
        }
    }
}
