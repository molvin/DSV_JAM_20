using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance;

    public int Level = -1;

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

}
