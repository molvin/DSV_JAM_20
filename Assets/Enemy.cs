using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int Score = 5;
    public int Multi = 1;

    private void Start()
    {
        GetComponent<Health>().onDeath += () =>
        {
            PersistentData.Instance.IncreaseScore(Score);
            PersistentData.Instance.IncreaseMultiplier(Multi);
        };
    }
}
