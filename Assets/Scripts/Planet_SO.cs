using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Planet_SO", menuName = "Planet Game/Planet_SO", order = 2)]
public class PlanetSO : ScriptableObject
{
    public Planet_Type type;
    public float MinMass;
    public float MaxMass;
    public float MinScale;
    public float MaxScale;
    public AudioClip collisionSound;
    public GameObject optional_prefab;
    [SerializeField]
    public List<PlanetTile> TilesAvailable;
}
