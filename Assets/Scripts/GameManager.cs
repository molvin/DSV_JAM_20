using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerPrefab;

    public void Start()
    {
        Player.Instance.GetComponent<Health>().onDeath += GameOver;
        LoadLevel();
        StartGame();
    }

    public void LoadLevel()
    {

    }

    public void StartGame()
    {

    }

    public void GameOver()
    {

    }
}
