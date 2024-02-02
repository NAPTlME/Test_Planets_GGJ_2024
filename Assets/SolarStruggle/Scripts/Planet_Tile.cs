using UnityEngine;
using System.Collections.Generic;

public enum Planet_Type
{
    earth,
    moon,
    gas
}
[CreateAssetMenu(fileName = "PlanetTile", menuName = "Planet Game/Planet_Tile", order = 2)]
public class PlanetTile : ScriptableObject
{
    public Planet_Type type;
    public Material TileMaterial;
    [SerializeField]
    public List<GameObject> AllowedEntities;
}
