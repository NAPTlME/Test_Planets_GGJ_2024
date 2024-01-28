using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
public enum StatsPlanetType
{
    SMALL,
    MEDIUM,
    LARGE
}
public class StatsManager : MonoBehaviour
{

    private static StatsManager instance = null;
    private int score = 0;
    private int killedResidents = 0;
    private int planetsUsed = 0;
    public TMP_Text StatsText;

    private Dictionary<StatsPlanetType, int> PlanetTypeToPoints = new Dictionary<StatsPlanetType, int>() {
        {StatsPlanetType.SMALL, 10},
        {StatsPlanetType.MEDIUM, 30},
        {StatsPlanetType.LARGE, 100},
    };

    private Dictionary<StatsPlanetType, int> PlanetTypeToResidents = new Dictionary<StatsPlanetType, int>() {
        {StatsPlanetType.SMALL, 1000},
        {StatsPlanetType.MEDIUM, 3000},
        {StatsPlanetType.LARGE, 1000000},
    };

    public Dictionary<PlanetType, StatsPlanetType> PlanetTypePrefabToEnum = new Dictionary<PlanetType, StatsPlanetType>();


    public static StatsManager getInstance()
    {
        return instance;
    }

    public void PlanetLaunched(StatsPlanetType type)
    {
        Debug.Log("Launched planet of stats type:");
        Debug.Log(type);
        score += PlanetTypeToPoints[type];
        planetsUsed += 1;
    }

    public void KillResidents(PlanetType type)
    {
        var enumType = PlanetTypePrefabToEnum[type];
        killedResidents += PlanetTypeToResidents[enumType];
    }

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (StatsText != null)
        {
            StatsText.text = "Planets used: " + planetsUsed + "\nResidents Killed: " + killedResidents + "\nScore: " + score;
        }
    }
}
