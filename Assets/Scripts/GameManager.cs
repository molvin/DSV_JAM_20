﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject PlayerPrefab;
    public int Segments;
    public float Sporadic;
    public float NoiseScale;
    public int BoidsPerSegment;
    public TunnelMaker Tunnel;
    public GameObject VictoryZonePrefab;

    [Header("UI")]
    public GameObject LoadingUI;
    public Image FadeImage;
    public TextMeshProUGUI LoadingText;
    public TextMeshProUGUI ProgressText;
    public GameObject VictoryUI;
    public float FadeOutTime;
    public float FadeInTime;

    private void Awake()
    {
        Instance = this;
    }
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
            LoadingUI.SetActive(true);
            LoadingText.gameObject.SetActive(true);
            ProgressText.gameObject.SetActive(true);
            FadeImage.color = FadeImage.color.withAlpha(1.0f);

            Player.Instance.gameObject.SetActive(false);
            ProgressText.text = $"Loaded 0/{Segments} Segments";
            Tunnel.Progress += (i) => { ProgressText.text = $"Loaded {i+1}/{Segments} Segments"; };
            yield return Tunnel.createLevelSLowLike(Segments, Sporadic, NoiseScale);

            SplineNoise3D.Spline end = SplineNoise3D.SplineLine[SplineNoise3D.SplineLine.Count - 1];
            GameObject go = Instantiate(VictoryZonePrefab, end.pos, Quaternion.identity);
            go.transform.localScale = new Vector3(end.radius, end.radius, end.radius);

            foreach (var s in SplineNoise3D.SplineLine)
            {
                BoidsManager.Spawn(s.pos, s.radius * 0.5f, BoidsPerSegment, Player.Instance.transform);
            }

            Vector3 forward = (SplineNoise3D.SplineLine[1].pos - SplineNoise3D.SplineLine[0].pos).normalized;
            Player.Instance.transform.position = SplineNoise3D.SplineLine[0].pos + forward * 2f;
            Player.Instance.SetForward(forward);
            Player.Instance.gameObject.SetActive(true);

            LoadingText.gameObject.SetActive(false);
            ProgressText.gameObject.SetActive(false);

            float t = 0.0f;
            while(t < FadeOutTime)
            {
                t += Time.unscaledDeltaTime;
                FadeImage.color = FadeImage.color.withAlpha(1f - t / FadeOutTime);
                yield return null;
            }
            LoadingUI.SetActive(false);
            Player.Instance.MovementMachine.TransitionTo<FlyingState>();
        }
    }

    public void StartGame()
    {

    }

    public void GameOver()
    {
        Debug.Log("You died");
        Player.Instance.gameObject.SetActive(false);

        StartCoroutine(DieRoutine());
        IEnumerator DieRoutine()
        {
            float t = 0.0f;
            while(t < 3)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            Vector3 forward = (SplineNoise3D.SplineLine[1].pos - SplineNoise3D.SplineLine[0].pos).normalized;
            Player.Instance.transform.position = SplineNoise3D.SplineLine[0].pos + forward * 2f;
            Player.Instance.SetForward(forward);
            Player.Instance.gameObject.SetActive(true);
            Player.Instance.MovementMachine.TransitionTo<IdleState>();
            t = 0.0f;
            while (t < 3)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            Player.Instance.MovementMachine.TransitionTo<FlyingState>();

        }


    }

    public void Win()
    {
        Debug.Log("YOU WIN THE LEVEL");
        Player.Instance.gameObject.SetActive(false);

        StartCoroutine(VictoryCoroutine());
        IEnumerator VictoryCoroutine()
        {

            yield return null;
        }
    }
}

public static class ExtendUI
{
    public static Color withAlpha(this Color color, float a)
    {
        color.a = a;
        return color;
    }
}