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

    const int MAX_RESIDENTS_KILLED = 1000000;
    private static StatsManager instance = null;
    private int score = 0;
    private int killedResidents = 0;
    private int planetsUsed = 0;
    public TMP_Text StatsText;
    public TMP_Text KilledResidentsTemporary;

    double tempResidentsKilledPromptHideFrameNumber = 0f;

    int remainingToKillBeforeGameOver = MAX_RESIDENTS_KILLED;

    private Dictionary<StatsPlanetType, int> PlanetTypeToPoints = new Dictionary<StatsPlanetType, int>() {
        {StatsPlanetType.SMALL, 35},
        {StatsPlanetType.MEDIUM, 50},
        {StatsPlanetType.LARGE, 100},
    };

    private Dictionary<StatsPlanetType, int> PlanetTypeToResidents = new Dictionary<StatsPlanetType, int>() {
        {StatsPlanetType.SMALL, 35000},
        {StatsPlanetType.MEDIUM, 50000},
        {StatsPlanetType.LARGE, 100000},
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
        var newKilledResidents = PlanetTypeToResidents[enumType];
        killedResidents += newKilledResidents;
        remainingToKillBeforeGameOver -= newKilledResidents;
        score -= PlanetTypeToPoints[enumType];
        if (remainingToKillBeforeGameOver <= 0)
        {
            GlobalManager.getInstance().GameOver(score);
            KilledResidentsTemporary.text = "You killed " + MAX_RESIDENTS_KILLED + " due to your clumsiness. The gods noticed. You are fired.";
            tempResidentsKilledPromptHideFrameNumber = double.PositiveInfinity;
        }
        else
        {
            KilledResidentsTemporary.text = newKilledResidents + " RESIDENTS DIED !!\n" + remainingToKillBeforeGameOver + " RESIDENTS REMAINING BEFORE THE GODS NOTICE.";
            tempResidentsKilledPromptHideFrameNumber = Time.frameCount + 120; // ~1s
        }
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
        if (tempResidentsKilledPromptHideFrameNumber < Time.frameCount)
        {
            KilledResidentsTemporary.text = "";
        }
    }
}
