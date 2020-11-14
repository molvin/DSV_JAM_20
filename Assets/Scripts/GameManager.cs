using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public int Segments;
    public float Sporadic;
    public float NoiseScale;
    public int BoidsPerSegment;
    public TunnelMaker Tunnel;
    public void Start()
    {
        Player.Instance.GetComponent<Health>().onDeath += GameOver;
        LoadLevel();
        StartGame();
    }

    public void LoadLevel()
    {
        StartCoroutine(buildLevel());
        IEnumerator buildLevel()
        {
            Player.Instance.gameObject.SetActive(false);

            yield return Tunnel.createLevelSLowLike(Segments, Sporadic, NoiseScale);

            Player.Instance.transform.position = SplineNoise3D.SplineLine[0].pos;
            Player.Instance.Velocity = (SplineNoise3D.SplineLine[1].pos - SplineNoise3D.SplineLine[0].pos).normalized;

            Player.Instance.gameObject.SetActive(true);

            foreach (var s in SplineNoise3D.SplineLine)
            {
                Debug.Log(s.pos);
                BoidsManager.Spawn(s.pos, s.radius, BoidsPerSegment);
            }
        }
    }

    public void StartGame()
    {

    }

    public void GameOver()
    {

    }
}
