using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static int RealMod(int a, int b)
    {
        return (a % b + b) % b;
    }

    public static string AddOrdinal(int num)
    {
        if (num <= 0) return num.ToString();

        switch (num % 100)
        {
            case 11:
            case 12:
            case 13:
                return num + "th";
        }

        switch (num % 10)
        {
            case 1:
                return num + "st";
            case 2:
                return num + "nd";
            case 3:
                return num + "rd";
            default:
                return num + "th";
        }
    }

    public static string ScoreboardTimer(float timer)
    {
        int mins = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - mins * 60);
        return string.Format("{0:0}:{1:00}", mins, seconds);
    }
}
