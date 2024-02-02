using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Planet_SO", menuName = "Planet Game/Planet_SO", order = 2)]
public class PlanetType : ScriptableObject
{
    public float minMass;
    public float maxMass;
    public float minScale;
    public float maxScale;
    public int pointValue;
    public int population;
    public AudioClip collisionSound;
    [SerializeField]
    public List<PlanetTile> tilesAvailable;
    public GameObject modelOverride;
}
