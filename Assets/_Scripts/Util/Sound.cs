using System;
using UnityEngine;

[Serializable]
public class Sound
{
    [SerializeField] private AudioClip clip;
    [SerializeField] [Range(0, 1)] private float volume = 1;

    public AudioClip Clip => clip;
    public float Volume => volume;
}