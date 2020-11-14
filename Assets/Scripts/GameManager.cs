using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
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
    public GameObject VictoryUI;
    public float FadeOutTime;
    public float FadeInTime;


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
            FadeImage.color = FadeImage.color.withAlpha(1.0f);

            Player.Instance.gameObject.SetActive(false);

            yield return Tunnel.createLevelSLowLike(Segments, Sporadic, NoiseScale);

            SplineNoise3D.Spline end = SplineNoise3D.SplineLine[SplineNoise3D.SplineLine.Count - 1];
            GameObject go = Instantiate(VictoryZonePrefab, end.pos, Quaternion.identity);
            go.transform.localScale = new Vector3(end.radius, end.radius, end.radius);

            foreach (var s in SplineNoise3D.SplineLine)
            {
                BoidsManager.Spawn(s.pos, s.radius * 0.5f, BoidsPerSegment);
            }


            Vector3 forward = (SplineNoise3D.SplineLine[1].pos - SplineNoise3D.SplineLine[0].pos).normalized;
            Debug.Log(forward);
            Player.Instance.transform.position = SplineNoise3D.SplineLine[0].pos + forward * 2f;
            Player.Instance.SetForward(forward);
            Player.Instance.gameObject.SetActive(true);

            LoadingText.gameObject.SetActive(false);
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