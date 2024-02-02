using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class JokeSpec
{
    public string text = "";
    public bool used = false;

    public int threshold;

    public JokeSpec(int threshold, string text)
    {
        this.text = text;
        this.threshold = threshold;
    }
}

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    public RippleGridAnim rippleGridAnim;
    const int MAX_RESIDENTS_KILLED = 1000000;
    private int score = 0;
    private int killedResidents = 0;
    private int planetsUsed = 0;
    private float secondsSinceLastYear = 0;
    private int years = 0;

    public TMP_Text PlanentsKilledText;
    public TMP_Text JokesTextee;
    public TMP_Text ResidentsKilledText;
    public TMP_Text ScoreText;
    public TMP_Text YearText;
    public TMP_Text KilledResidentsTemporary;
    private AudioSource audioSource;

    private JokeSpec[] jokes = new JokeSpec[] {
        new JokeSpec((int)(0.3 * MAX_RESIDENTS_KILLED), "You have a long way to go, apprentice."),
        new JokeSpec((int)(0.6 * MAX_RESIDENTS_KILLED), "More tactful, you must be."),
        new JokeSpec((int)(0.7 * MAX_RESIDENTS_KILLED), "Chuck Norris' grandson lived on this planet!"),
        new JokeSpec((int)(0.9 * MAX_RESIDENTS_KILLED), "Hey! Those are real people!"),
    };

    double tempResidentsKilledPromptHideFrameNumber = 0f;
    double tempJokesKilledPromptHideFrameNumber = 0f;

    int remainingToKillBeforeGameOver = MAX_RESIDENTS_KILLED;

    //public Dictionary<Planet_Type, StatsPlanetType> PlanetTypePrefabToEnum = new Dictionary<Planet_Type, StatsPlanetType>();

    public void PlanetLaunched(PlanetType planetType)
    {
        //Debug.Log("Launched planet of stats type:");
        //Debug.Log(type);
        score += planetType.pointValue;
        planetsUsed += 1;
    }

    public void LostPlanet(PlanetType type)
    {
        Debug.Log("Planet lost!");
        Debug.Log(type);
        KillResidents(type, true);
    }

    public void KillResidents(PlanetType type, bool overrideAsLost = false)
    {
        rippleGridAnim.increaseTemp();
        var newKilledResidents = type.population;
        killedResidents += newKilledResidents;
        remainingToKillBeforeGameOver -= newKilledResidents;
        score -= type.pointValue;
        if (!overrideAsLost)
        {
            audioSource.Play();
        }

        foreach (var joke in jokes)
        {
            if (remainingToKillBeforeGameOver <= joke.threshold && !joke.used)
            {
                JokesTextee.text = joke.text;
                joke.used = true;
                tempJokesKilledPromptHideFrameNumber = Time.frameCount + 400;
                break;
            }
        }


        if (remainingToKillBeforeGameOver <= 0)
        {
            GlobalManager.Instance.GameOver(score);
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
            tempResidentsKilledPromptHideFrameNumber = Time.frameCount + 350; // ~1s
        }
    }

    private void Awake()
    {
        // If there is an instance and it's not me, delete myself
        if (Instance is not null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        secondsSinceLastYear = 0;
        audioSource = GetComponent<AudioSource>();
        years = 0;
    }

    private void Update()
    {
        secondsSinceLastYear += Time.deltaTime;
        if (secondsSinceLastYear > 5)
        {
            secondsSinceLastYear -= 5;
            years++;
        }

        if (PlanentsKilledText != null)
        {
            PlanentsKilledText.text = planetsUsed.ToString("D6");
        }
        if (ResidentsKilledText != null)
        {
            ResidentsKilledText.text = killedResidents.ToString("D6");
        }
        if (ScoreText != null)
        {
            ScoreText.text = score.ToString("D6");
        }
        if (YearText != null)
        {
            YearText.text = years.ToString("D6");
        }


        if (tempResidentsKilledPromptHideFrameNumber < Time.frameCount)
        {
            KilledResidentsTemporary.text = "";
        }
        if (tempJokesKilledPromptHideFrameNumber < Time.frameCount)
        {
            JokesTextee.text = "";
        }
    }
}
