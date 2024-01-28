using UnityEngine;

[CreateAssetMenu(fileName = "PlanetType", menuName = "Planet Game/Planet Type", order = 1)]
public class PlanetType : ScriptableObject
{
    public float mass;
    public float scale;
    public Material tileMaterial;
    public AudioClip collisionSound;
    
}
