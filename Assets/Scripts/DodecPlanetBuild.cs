using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DodecPlanetBuild : MonoBehaviour
{
    public GameObject CorePrefab;
    public GameObject TilePrefab;
    public Mesh DodecaHedronColliderPrefab;
    public List<GameObject> EntityCollections;
    public float entitySpawnChance = 0.3f;
    public float objectSpawnChance = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        Build();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Build()
    {
        var planet = new GameObject("spawned_planet");
        planet.transform.position = Vector3.zero; // probably not necessary
        // rigidbody, mass
        //planet radius
        // create empty to hold the planet
        // get mesh collider
        // get the core
        // get 12 instances of the tiles
        var core = Instantiate(CorePrefab, planet.transform);
        var tiles = Enumerable.Range(0, 12).Select( sel =>
         {
             // for now just using one prefab. potentially will select from a collection
             //var index = Mathf.FloorToInt(Random.value * TilePrefabs.Count);
             return Instantiate(TilePrefab, planet.transform);
         }).ToList();

        // negative x is the bottom/flip face
        // the interior angles add up to 540 and are 108 each
        // top stays
        // duplicate from the top
        // next layer is 60 degrees in the Z axis
        Dictionary<int, GameObject> entityCreations = Enumerable.Range(0, 12).Where(wh => Random.value <= entitySpawnChance).ToDictionary(i =>i, i =>
         {
             var index = Mathf.FloorToInt(Random.value * EntityCollections.Count);
             return Instantiate(EntityCollections.ElementAt(index), planet.transform);
         });

        for (int i = 1; i < 6; i++)
        {
            // re: 63.4f. I don't know if there is some skew or not, but this is not typical (I think?).
            var rotation = Quaternion.AngleAxis(63.4f, Vector3.forward);
            rotation *= Quaternion.AngleAxis(180, Vector3.up);
            rotation = Quaternion.AngleAxis(72 * (i-1), Vector3.up) * rotation;

            tiles.ElementAt(i).transform.rotation = rotation;

            if (entityCreations.ContainsKey(i))
            {
                entityCreations[i].transform.rotation = rotation;
            }
        }
        // next layer is 240 degrees in the z axis
        for (int i = 6; i < 11; i++)
        {
            var rotation = Quaternion.AngleAxis(180 + 63.4f, Vector3.forward);
            rotation *= Quaternion.AngleAxis(180, Vector3.up);
            rotation = Quaternion.AngleAxis(72 * (i - 1), Vector3.up) * rotation;

            tiles.ElementAt(i).transform.rotation = rotation;

            if (entityCreations.ContainsKey(i))
            {
                entityCreations[i].transform.rotation = rotation;
            }
        }

        // last layer is 180 (x) from top 
        // and a 180 rotation on y? maybe not?

        tiles.ElementAt(11).transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);

        if (entityCreations.ContainsKey(11))
        {
            entityCreations[11].transform.rotation = Quaternion.AngleAxis(180, Vector3.forward); ;
        }
    }
}
