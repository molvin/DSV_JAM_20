﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance;

    public int Level = -1;
    public int Lives = 3;
    public int Score = 0;
    public int Multiplier = 1;
    public string LatestScoreSource = "";
    private AudioSource dedSound;
    private void Awake()
    {
        dedSound = GetComponent<AudioSource>();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            DestroyImmediate(gameObject);
    }

    public void IncreaseScore(int score, string source = "")
    {
        Score += score * Multiplier;
        LatestScoreSource = source;
        if (PlayerPrefs.GetInt("Highscore", 0) < Score)
            PlayerPrefs.SetInt("Highscore", Score);
    }
    public void IncreaseMultiplier(int multi)
    {
        Multiplier += multi;
    }

    public void ResetMultiplier()
    {
        if (Multiplier > 2)
        {
            dedSound.Play();
        }
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
