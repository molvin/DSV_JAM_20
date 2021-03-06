﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioReverbZone ReverbZone;
    public AudioSource BoostDown;

    private void Start()
    {
        Player.FlyingState.OnBoostStart += () => ReverbZone.reverbPreset = AudioReverbPreset.Psychotic;
        Player.FlyingState.OnBoostEnd += () =>
        {
            ReverbZone.reverbPreset = AudioReverbPreset.Cave;
            BoostDown.Play();
        };
    }
}
