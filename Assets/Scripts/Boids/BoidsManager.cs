using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsManager : MonoBehaviour
{
    public GameObject BoidPrefab;
    public List<Boid> Boids = new List<Boid>();
    public static BoidsManager Instance;

    private void Start()
    {
        Instance = this;
    }

    public static void Spawn(Vector3 Point, float SpawnRadius, int NumBoids)
    {
        for (int i = 0; i < NumBoids; i++)
        {
            Instance.Boids.Add(Instantiate(Instance.BoidPrefab, Random.insideUnitSphere * SpawnRadius, Random.rotation).GetComponent<Boid>());
        }
    }
}
