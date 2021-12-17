using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuCleonSpawner : MonoBehaviour
{
    public float timeBetweenSpawns;
    public float spawnDistance;
    public Nucleon[] nucleonPrefabs;

    float timeSinceLastSpawn;

    void FixedUpdate()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if(timeSinceLastSpawn >= timeBetweenSpawns)
        {
            timeSinceLastSpawn -= timeBetweenSpawns;
            SpawnNucleon();
        }
    }

    void SpawnNucleon()
    {
        Nucleon prefab = nucleonPrefabs[Random.Range(0, nucleonPrefabs.Length)];
        Nucleon spwan = Instantiate<Nucleon>(prefab);
        spwan.transform.localPosition = Random.onUnitSphere * spawnDistance;
    }
}
