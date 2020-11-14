using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXDisabler : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(true);

    }
    private void OnEnable()
    {
        CancelInvoke("Disableself");
        GetComponent<ParticleSystem>().Play();
        Invoke("Disableself", 1.5f);

    }
    private void Disableself()
    {
        gameObject.SetActive(false);
    }
}
