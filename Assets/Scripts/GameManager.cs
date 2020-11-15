using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

using UnityEngine.Rendering.PostProcessing;
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
    public TextMeshProUGUI LevelText;
    public GameObject VictoryUI;
    public float FadeOutTime;
    public float FadeInTime;

    public Action OnStartedLoad;
    
    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        
        Player.Instance.GetComponent<Health>().onDeath += GameOver;
        LoadLevel();
    }

    public void LoadLevel()
    {
        OnStartedLoad?.Invoke();
        StartCoroutine(buildLevel());
        IEnumerator buildLevel()
        {
            PersistentData.Instance.Level++;

            PlayerGUI.Instance.Disable();
            LoadingUI.SetActive(true);
            LoadingText.gameObject.SetActive(true);
            ProgressText.gameObject.SetActive(true);
            LevelText.gameObject.SetActive(true);
            FadeImage.color = Color.black.withAlpha(1.0f);

            Player.Instance.gameObject.SetActive(false);
            ProgressText.text = $"Loaded 0/{Segments} Segments";
            LevelText.text = $"Level {PersistentData.Instance.Level}";
            Tunnel.Progress += (i) => { ProgressText.text = $"Loaded {i+1}/{Segments} Segments"; };
            yield return Tunnel.createLevelSLowLike(Segments, Sporadic, NoiseScale);

            SplineNoise3D.Spline end = SplineNoise3D.SplineHole[SplineNoise3D.SplineHole.Count - 1];
            GameObject go = Instantiate(VictoryZonePrefab, end.pos, Quaternion.identity);
            go.transform.localScale = new Vector3(end.radius, end.radius, end.radius);

            int count = 0;
            foreach (var s in SplineNoise3D.SplineHole)
            {
                if(count > 2)
                {
                    BoidsManager.Spawn(s.pos, s.radius * 0.5f, BoidsPerSegment, Player.Instance.transform);

                }
                count++;
            }

            Vector3 forward = (SplineNoise3D.SplineHole[1].pos - SplineNoise3D.SplineHole[0].pos).normalized;
            Player.Instance.transform.position = SplineNoise3D.SplineHole[0].pos + forward * 2f;
            Player.Instance.SetForward(forward);
            Player.Instance.gameObject.SetActive(true);
            PlayerGUI.Instance.Enable();

            LoadingText.gameObject.SetActive(false);
            ProgressText.gameObject.SetActive(false);
            LevelText.gameObject.SetActive(false);


            float t = 0.0f;
            while(t < FadeOutTime)
            {
                t += Time.unscaledDeltaTime;
                FadeImage.color = Color.black.withAlpha(1f - t / FadeOutTime);
                yield return null;
            }
            LoadingUI.SetActive(false);
            Player.Instance.MovementMachine.TransitionTo<FlyingState>();
          
        }
    }

    public void GameOver()
    {
        Debug.Log("You died");
        Player.Instance.gameObject.SetActive(false);
        var volume = Camera.main.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out ChromaticAberration chromatic);
        chromatic.intensity.value = 0.0f;

        OnStartedLoad?.Invoke();
        PersistentData.Instance.DecreaseLives();

        if(PersistentData.Instance.Lives <= 0)
        {
            PersistentData.Instance.ResetLives();
            PersistentData.Instance.ResetScore();
        }
        PersistentData.Instance.ResetMultiplier();


        StartCoroutine(DieRoutine());
        IEnumerator DieRoutine()
        {
            LoadingUI.SetActive(true);
            PlayerGUI.Instance.Disable();

            float t = 0.0f;
            while(t < 3)
            {
                t += Time.unscaledDeltaTime;
                FadeImage.color = Color.black.withAlpha(t / 3.0f);
                yield return null;
            }
            FadeImage.color = Color.black.withAlpha(1.0f);

            Vector3 forward = (SplineNoise3D.SplineHole[1].pos - SplineNoise3D.SplineHole[0].pos).normalized;
            Player.Instance.transform.position = SplineNoise3D.SplineHole[0].pos + forward * 2f;
            Player.Instance.SetForward(forward);
            Player.Instance.gameObject.SetActive(true);
            Player.Instance.MovementMachine.TransitionTo<IdleState>();

            BoidsManager.ClearBoids();
            int count = 0;
            foreach (var s in SplineNoise3D.SplineHole)
            {
                if(count > 2)
                    BoidsManager.Spawn(s.pos, s.radius * 0.5f, BoidsPerSegment, Player.Instance.transform);
                count++;
            }

            t = 0.0f;
            PlayerGUI.Instance.Enable();

            while (t < 3)
            {
                t += Time.unscaledDeltaTime;
                FadeImage.color = Color.black.withAlpha(1f - t / 3.0f);
                yield return null;
            }
            FadeImage.color = Color.black.withAlpha(0.0f);

            Player.Instance.MovementMachine.TransitionTo<FlyingState>();
            LoadingUI.SetActive(false);

        }


    }

    public void Win()
    {
        Debug.Log("YOU WIN THE LEVEL");
        Player.Instance.gameObject.SetActive(false);
        PlayerGUI.Instance.Disable();
        StartCoroutine(VictoryCoroutine());
        IEnumerator VictoryCoroutine()
        {
            float t = 0.0f;
            LoadingUI.SetActive(true);

            while (t < 3)
            {
                t += Time.unscaledDeltaTime;
                FadeImage.color = Color.black.withAlpha(t / 3.0f);
                yield return null;
            }

            BoidsManager.ClearBoids();
            PlayerGUI.Instance.Disable();
            SceneManager.LoadScene(1);
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