using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DodecPlanetBuild : MonoBehaviour
{
    public GameObject EmptyPlanetPrefab;
    public GameObject CorePrefab;
    public GameObject TilePrefab;
    public GameObject GasGiantPrefab;
    public float objectSpawnChance = 0.3f;
    [SerializeField]
    public List<PlanetSO> scriptable_planets;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public (GameObject, Planet_Type) Build(PlanetSO planet_so = null)
    {
        if (planet_so == null)
        {
            var index = Mathf.FloorToInt(Random.value * scriptable_planets.Count);
            planet_so = scriptable_planets.ElementAt(index);
        }
        var lerpAmt = Random.value;
        var mass = Mathf.Lerp(planet_so.MinMass, planet_so.MaxMass, lerpAmt);
        var scale = Mathf.Lerp(planet_so.MinScale, planet_so.MaxScale, lerpAmt);
        var planet = Instantiate(EmptyPlanetPrefab);
        planet.transform.position = Vector3.zero; // probably not necessary
        // rigidbody, mass
        Rigidbody rbody = planet.GetComponent<Rigidbody>();
        rbody.mass = mass;
        planet.transform.localScale = Vector3.one * scale;
        Planet planetBehavior = planet.GetComponent<Planet>();
        // audio source
        AudioSource audio = planet.GetComponent<AudioSource>();
        audio.clip = planet_so.collisionSound;
        //planet radius
        if (planet_so.type == Planet_Type.gas)
        {
            var mesh = Instantiate(GasGiantPrefab, planet.transform);
            return (planet, planet_so.type);
        } else
        {
            // get the core
            // get 12 instances of the tiles
            var core = Instantiate(CorePrefab, planet.transform);
            // get random tiles from the possible tiles
            var tiles = Enumerable.Range(0, 12).Select(sel =>
            {
                var index = Mathf.FloorToInt(Random.value * planet_so.TilesAvailable.Count);
                return InstantiatePlanetTile(planet_so.TilesAvailable.ElementAt(index), planetBehavior);
            }).ToList();

            // negative x is the bottom/flip face
            // the interior angles add up to 540 and are 108 each
            // top stays
            // duplicate from the top
            // next layer is 60 degrees in the Z axis

            for (int i = 1; i < 6; i++)
            {
                // re: 63.4f. I don't know if there is some skew or not, but this is not typical (I think?).
                var rotation = Quaternion.AngleAxis(63.4f, Vector3.forward);
                rotation *= Quaternion.AngleAxis(180, Vector3.up);
                rotation = Quaternion.AngleAxis(72 * (i - 1), Vector3.up) * rotation;

                tiles.ElementAt(i).Item1.transform.rotation = rotation;

                if (tiles.ElementAt(i).Item2 != null)
                {
                    tiles.ElementAt(i).Item2.transform.rotation = rotation;
                }
            }
            // next layer is 240 degrees in the z axis
            for (int i = 6; i < 11; i++)
            {
                var rotation = Quaternion.AngleAxis(180 + 63.4f, Vector3.forward);
                rotation *= Quaternion.AngleAxis(180, Vector3.up);
                rotation = Quaternion.AngleAxis(72 * (i - 1), Vector3.up) * rotation;

                tiles.ElementAt(i).Item1.transform.rotation = rotation;

                if (tiles.ElementAt(i).Item2 != null)
                {
                    tiles.ElementAt(i).Item2.transform.rotation = rotation;
                }
            }

            // last layer is 180 (x) from top 
            // and a 180 rotation on y? maybe not?

            tiles.ElementAt(11).Item1.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);

            if (tiles.ElementAt(11).Item2 != null)
            {
                tiles.ElementAt(11).Item2.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
            }
            return (planet, planet_so.type);
        }
    }
    private (GameObject, GameObject) InstantiatePlanetTile(PlanetTile tileInfo, Planet homePlanet)
    {
        var tile = Instantiate(TilePrefab, homePlanet.transform);
        GameObject entity = null;
        tile.GetComponentInChildren<MeshRenderer>().material = tileInfo.TileMaterial;
        if (tileInfo.AllowedEntities.Count > 0 && Random.value <= objectSpawnChance)
        {
            var index = Mathf.FloorToInt(Random.value * tileInfo.AllowedEntities.Count);
            entity = Instantiate(tileInfo.AllowedEntities.ElementAt(index), homePlanet.transform);
            // check if entity is one that has the Objects_On_Planet component
            var objectInfo = entity.GetComponentsInChildren<Objects_On_Planet>();
            if (objectInfo.Count() > 0)
            {
                objectInfo.ToList().ForEach(x =>
                {
                    x.homePlanet = homePlanet;
                    x.planets.Add(homePlanet);
                });
            }
        }
        return (tile, entity);
    }
}