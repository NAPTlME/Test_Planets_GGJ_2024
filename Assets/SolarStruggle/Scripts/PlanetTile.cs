using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlanetTile", menuName = "Planet Game/Planet_Tile", order = 2)]
public class PlanetTile : ScriptableObject
{
    public Material TileMaterial;
    [SerializeField]
    public List<GameObject> AllowedEntities;
}
