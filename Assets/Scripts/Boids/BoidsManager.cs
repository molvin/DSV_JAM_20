using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsManager : MonoBehaviour
{
    public GameObject BoidPrefab;
    public List<BoidComp> Boids = new List<BoidComp>();
    public static BoidsManager Instance;

    public int ThreadGroupSize = 1024;
    public BoidSettings Settings;
    public ComputeShader Compute;

    private void Awake()
    {
        Instance = this;
        Spawn(Vector3.zero, 50, 500);
    }

    private void Update()
    {
        int NumBoids = Boids.Count;
        if (NumBoids == 0)
        {
            return;
        }
        var boidData = new BoidData[NumBoids];

        for (int i = 0; i < NumBoids; i++)
        {
            boidData[i].Position = Boids[i].transform.position;
            boidData[i].Direction = Boids[i].transform.forward;
        }

        ComputeBuffer Buffer = new ComputeBuffer(NumBoids, BoidData.Size);
        Buffer.SetData(boidData);

        Compute.SetBuffer(0, "boids", Buffer);
        Compute.SetInt("numBoids", Boids.Count);
        Compute.SetFloat("viewRadius", Settings.PerceptionRadius);
        Compute.SetFloat("avoidRadius", Settings.AvoidanceRadius);

        int ThreadGroups = Mathf.CeilToInt(NumBoids / (float)ThreadGroupSize);
        Compute.Dispatch(0, ThreadGroups, 1, 1);

        Buffer.GetData(boidData);

        for (int i = 0; i < Boids.Count; i++)
        {
            Boids[i].AverageFlockHeading = boidData[i].FlockHeading;
            Boids[i].CentreOfFlockmates = boidData[i].FlockCentre;
            Boids[i].AverageAvoidanceHeading = boidData[i].AvoidanceHeading;
            Boids[i].NumPerceivedFlockmates = boidData[i].NumFlockmates;

            Boids[i].UpdateBoy();
        }
        Buffer.Release();
    }

    public static void Spawn(Vector3 Point, float SpawnRadius, int NumBoids)
    {
        for (int i = 0; i < NumBoids; i++)
        {
            Instance.Boids.Add(Instantiate(Instance.BoidPrefab, Random.insideUnitSphere * SpawnRadius, Random.rotation).GetComponent<BoidComp>());
        }
    }

    public struct BoidData
    {
        public Vector3 Position;
        public Vector3 Direction;

        public Vector3 FlockHeading;
        public Vector3 FlockCentre;
        public Vector3 AvoidanceHeading;
        public int NumFlockmates;

        public static int Size
        {
            get { return sizeof(float) * 3 * 5 + sizeof(int); }
        }
    }
}
