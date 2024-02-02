using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Slideshow : MonoBehaviour
{
    public List<GameObject> slides;
    public int currentSlideIndex;
    public GlobalManager.Scenes nextScene;

    // Start is called before the first frame update
    void Start()
    {
        currentSlideIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(currentSlideIndex < slides.Count - 1)
            {
                NextSlide();
            }
            else
            {
                SceneManager.LoadScene((int)nextScene);
            }
        }

    }

    void NextSlide()
    {
        slides[currentSlideIndex].SetActive(false);
        currentSlideIndex++;
        slides[currentSlideIndex].SetActive(true);
    }
}
