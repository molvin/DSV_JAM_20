using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsManager : MonoBehaviour
{
    public int NumberOfBoidsToSpawn;
    public GameObject BoidPrefab;
    public float SpawnRadius;
    public StubPlayer Player;

    public List<Boid> Boids = new List<Boid>();

    public static BoidsManager Instance;

    private void Start()
    {
        Instance = this;
        
        for (int i = 0; i < NumberOfBoidsToSpawn; i++)
        {
            Boids.Add(Instantiate(BoidPrefab, Random.insideUnitSphere * SpawnRadius, Random.rotation).GetComponent<Boid>());
        }
    }
}
