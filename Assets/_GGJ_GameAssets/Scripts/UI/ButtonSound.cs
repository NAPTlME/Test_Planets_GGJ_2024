using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public void ButtonHover()
    {
        GameManager.PlayButtonSound(0);
    }

    public void ButtonClick()
    {
        GameManager.PlayButtonSound(1);
    }
}
