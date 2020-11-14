using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public void Start()
    {
        Player.Instance.GetComponent<Health>().onDeath += GameOver;
    }

    public void GameOver()
    {

    }
}
