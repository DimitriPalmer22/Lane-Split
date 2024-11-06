using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")] [SerializeField]
    private AudioSource sfxSource;

    [SerializeField] private AudioSource musicSource;

    [Header("Audio Clips")] [SerializeField]
    private AudioClip slomoClip;

    [SerializeField] private AudioClip[] musicClips;

    [Header("References")] [SerializeField]
    private CarRampHandler carRampHandler; // Reference to CarRampHandler

    //reference to the test player script
    [SerializeField] private TestPlayerScript playerScript;
    [Header("Settings")] [SerializeField] private float slowTimeScale = 0.5f; // Time scale for slow-motion
    [SerializeField] private float slowMotionDuration = 1.5f; //duration of the slow motion effect


    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        carRampHandler.OnRampEnter += OnRampEnter;
    }

    // Called when the ramp event is triggered
    public void OnRampEnter()
    {
        // Play the slow-motion sound effect
        PlayOneShot(slomoClip);
    }

    // Method to play sound effects
    public void PlayOneShot(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
        else
            Debug.LogWarning("SoundManager: Missing AudioSource or AudioClip.");
    }

    // Play music by index
    public void PlayMusic(int index)
    {
        if (index >= 0 && index < musicClips.Length)
        {
            musicSource.clip = musicClips[index];
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("SoundManager: Invalid music index.");
        }
    }

    // Stop the currently playing music
    public void StopMusic()
    {
        musicSource.Stop();
    }
}