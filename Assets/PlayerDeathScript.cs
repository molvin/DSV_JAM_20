using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathScript : MonoBehaviour
{
    private AudioSource m_AudioSource;
    private GameObject playerObject;
    void Start()
    {
        playerObject = Player.Instance.gameObject;
        playerObject.GetComponent<Health>().onDeath += Death;
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Death()
    {
        transform.position = playerObject.transform.position;
        m_AudioSource?.Play();
        transform.GetChild(0).gameObject.SetActive(true);
    }


}
