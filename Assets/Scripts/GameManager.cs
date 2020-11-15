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
    public TunnelMaker Tunnel;
    public GameObject VictoryZonePrefab;

    //Generation settings
    [Header("GENERATION SETTINGS!½!!!")]
    public Vector2Int SegmentsMinMax;
    public Vector2 SporadicMinMax;
    public Vector2 NoiseScaleMinMax;
    public Vector2Int BoidsPerSegmentMinMax;
    public Vector2 CaveWallAmountMinMax;
    public Vector2 InternalCaveAmountMinMax;
    public Vector2 InternalCaveNoiseMinMax;
    public Vector2 HoleSizeMinMax;
    private int _BoidsPerSegment;

    [Header("UI")]
    public GameObject LoadingUI;
    public Image FadeImage;
    public TextMeshProUGUI LoadingText;
    public TextMeshProUGUI ProgressText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI HighScoreText;
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
        
        //Level data
        float level = PersistentData.Instance.Level/10f;
        level = level > 1f ? 1f : level;

        //Segs
        int Segments = (int)Mathf.Lerp(SegmentsMinMax.x, SegmentsMinMax.y, level);
        //Sporadics
        float SporadicMax = Mathf.Lerp(SporadicMinMax.x, SporadicMinMax.y, Mathf.Clamp01(level + 0.3f));
        float Sporadic = UnityEngine.Random.Range(SporadicMinMax.x, SporadicMax);
        //Noise size
        float NoiseScale = UnityEngine.Random.Range(NoiseScaleMinMax.x, NoiseScaleMinMax.y);
        //Boids
        _BoidsPerSegment = (int)Mathf.Lerp(BoidsPerSegmentMinMax.x, BoidsPerSegmentMinMax.y, level);
        //Walls
        float CaveWallAmountMax = Mathf.Lerp(CaveWallAmountMinMax.x, CaveWallAmountMinMax.y, Mathf.Clamp01(level + 0.5f));
        float CaveWallAmount = UnityEngine.Random.Range(CaveWallAmountMinMax.x, CaveWallAmountMax);
        //Ohno things in the way
        float InternalCaveAmountMax = Mathf.Lerp(InternalCaveAmountMinMax.x, InternalCaveAmountMinMax.y, Mathf.Clamp01(level + 0.5f));
        float InternalCaveAmount = UnityEngine.Random.Range(InternalCaveAmountMinMax.x, InternalCaveAmountMax);
        //Ohno things in the way noise
        float InternalCaveNoise = UnityEngine.Random.Range(InternalCaveNoiseMinMax.x, InternalCaveNoiseMinMax.y);
        //Safety hole
        float HoleSize = UnityEngine.Random.Range(HoleSizeMinMax.x, HoleSizeMinMax.y);

        StartCoroutine(buildLevel());
        IEnumerator buildLevel()
        {
            PersistentData.Instance.Level++;
            PlayerGUI.Instance.Disable();
            LoadingUI.SetActive(true);
            LoadingText.gameObject.SetActive(true);
            ProgressText.gameObject.SetActive(true);
            HighScoreText.gameObject.SetActive(true);
            LevelText.gameObject.SetActive(true);
            FadeImage.color = Color.black.withAlpha(1.0f);

            Player.Instance.gameObject.SetActive(false);
            ProgressText.text = $"Generated 0/{Segments} Segments";
            LevelText.text = $"Level {PersistentData.Instance.Level}";
            HighScoreText.text = $"Highscore: {PlayerPrefs.GetInt("Highscore", 0)}";
            Tunnel.Progress += (i) => { ProgressText.text = $"Loaded {i+1}/{Segments} Segments"; };
            yield return Tunnel.createLevelSLowLike(Segments, Sporadic, NoiseScale, CaveWallAmount, InternalCaveAmount, InternalCaveNoise, HoleSize);

            SplineNoise3D.Spline end = SplineNoise3D.SplineLine[SplineNoise3D.SplineLine.Count - 2];
            GameObject go = Instantiate(VictoryZonePrefab, end.pos, Quaternion.identity);
            go.transform.localScale = new Vector3(end.radius, end.radius, end.radius);

            int count = 0;
            foreach (var s in SplineNoise3D.SplineHole)
            {
                if(count > 2 && count < SplineNoise3D.SplineHole.Count - 2)
                {
                    BoidsManager.Spawn(s.pos, s.radius * 0.5f, _BoidsPerSegment, Player.Instance.transform);
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
            HighScoreText.gameObject.SetActive(false);


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
            PersistentData.Instance.ResetMultiplier();
            PersistentData.Instance.ResetScore();
            PersistentData.Instance.Level = 0;
            LoadLevel();
            return;
        }


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
                    BoidsManager.Spawn(s.pos, s.radius * 0.5f, _BoidsPerSegment, Player.Instance.transform);
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