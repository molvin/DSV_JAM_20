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

    public static void Spawn(Vector3 Point, float SpawnRadius, int NumBoids, Transform Target = null)
    {
        for (int i = 0; i < NumBoids; i++)
        {
            BoidComp Boy = Instantiate(Instance.BoidPrefab, Point + Random.insideUnitSphere * SpawnRadius, Random.rotation).GetComponent<BoidComp>();
            Boy.Target = Target;
            Instance.Boids.Add(Boy);
        }
    }
    public static void ClearBoids()
    {
        foreach(BoidComp b in Instance.Boids)
        {
            Destroy(b.gameObject);
        }
        Instance.Boids.Clear();
    }

    public static void DeleteBoid(BoidComp Boy)
    {
        if (!Instance.Boids.Contains(Boy))
        {
            return;
        }
        Instance.Boids.Remove(Boy);
        Rigidbody rb = Boy.gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = Random.onUnitSphere * 15;
        Destroy(Boy);
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
