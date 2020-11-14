using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXDisabler : MonoBehaviour
{

    private void OnEnable()
    {
        if (!GetComponent<AudioSource>())
        {
            gameObject.SetActive(false);
            return;
        }
        if (GetComponent<AudioSource>().clip == null)
        {
            gameObject.SetActive(false);
            return;
        }
        CancelInvoke("DisableSelf");
        Invoke("DisableSelf", GetComponent<AudioSource>().clip.length);
    }
    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
