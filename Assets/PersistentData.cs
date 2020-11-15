using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance;

    public int Level = -1;
    public int Lives = 3;
    public int Score = 0;
    public int Multiplier = 1;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            DestroyImmediate(gameObject);
    }

    private void Update()
    {
        Debug.Log($"Score: {Score} Multi: {Multiplier}");
    }

    public void IncreaseScore(int score)
    {
        Score += score * Multiplier;
        if (PlayerPrefs.GetInt("Highscore", 0) < Score)
            PlayerPrefs.SetInt("Highscore", Score);
    }
    public void IncreaseMultiplier(int multi)
    {
        Multiplier += multi;
    }

    public void ResetMultiplier()
    {
        Multiplier = 1;
    }
    public void ResetScore()
    {
        Score = 0;
    }
    public void DecreaseLives()
    {
        Lives--;
    }
    public void ResetLives()
    {
        Lives = 3;
    }

}
