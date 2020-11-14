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
        Debug.Log(Time.time);
        CancelInvoke("Disableself");
        GetComponent<ParticleSystem>().Play();
        Invoke("Disableself", GetComponent<ParticleSystem>().main.startLifetime.constantMax);
        Debug.Log(GetComponent<ParticleSystem>().main.duration + GetComponent<ParticleSystem>().main.startLifetime.constantMax);
    }
    private void Disableself()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
