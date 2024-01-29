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
    public RippleGridAnim rippleGridAnim;
    const int MAX_RESIDENTS_KILLED = 1000000;
    private static StatsManager instance = null;
    private int score = 0;
    private int killedResidents = 0;
    private int planetsUsed = 0;
    public TMP_Text StatsText;
    public TMP_Text KilledResidentsTemporary;

    double tempResidentsKilledPromptHideFrameNumber = 0f;

    int remainingToKillBeforeGameOver = MAX_RESIDENTS_KILLED;

    private Dictionary<Planet_Type, int> PlanetTypeToPoints = new Dictionary<Planet_Type, int>() {
        {Planet_Type.moon, 35},
        {Planet_Type.earth, 50},
        {Planet_Type.gas, 100},
    };

    private Dictionary<Planet_Type, int> PlanetTypeToResidents = new Dictionary<Planet_Type, int>() { // to make this dependent on radius and only for earth-like?
        {Planet_Type.moon, 35000},
        {Planet_Type.earth, 50000},
        {Planet_Type.gas, 100000},
    };

    //public Dictionary<Planet_Type, StatsPlanetType> PlanetTypePrefabToEnum = new Dictionary<Planet_Type, StatsPlanetType>();


    public static StatsManager getInstance()
    {
        return instance;
    }

    public void PlanetLaunched(Planet_Type type)
    {
        //Debug.Log("Launched planet of stats type:");
        //Debug.Log(type);
        score += PlanetTypeToPoints[type];
        planetsUsed += 1;
    }

    public void LostPlanet(Planet_Type type)
    {
        Debug.Log("Planet lost!");
        Debug.Log(type);
        KillResidents(type, true);
    }

    public void KillResidents(Planet_Type type, bool overrideAsLost = false)
    {
        rippleGridAnim.increaseTemp();
        var newKilledResidents = PlanetTypeToResidents[type];
        killedResidents += newKilledResidents;
        remainingToKillBeforeGameOver -= newKilledResidents;
        score -= PlanetTypeToPoints[type];
        if (remainingToKillBeforeGameOver <= 0)
        {
            GlobalManager.getInstance().GameOver(score);
            KilledResidentsTemporary.text = "You killed " + MAX_RESIDENTS_KILLED + " due to your clumsiness. The gods noticed. You are fired.";
            tempResidentsKilledPromptHideFrameNumber = double.PositiveInfinity;
        }
        else
        {
            if (overrideAsLost)
            {
                KilledResidentsTemporary.text = newKilledResidents + " RESIDENTS LOST IN SPACE !!\n" + remainingToKillBeforeGameOver + " RESIDENTS REMAINING BEFORE THE GODS NOTICE.";
            }
            else
            {
                KilledResidentsTemporary.text = newKilledResidents + " RESIDENTS DIED !!\n" + remainingToKillBeforeGameOver + " RESIDENTS REMAINING BEFORE THE GODS NOTICE.";

            }
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
