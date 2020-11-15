using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager: MonoBehaviour
{
    public static MusicManager Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
