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
    private Sound[] musicClips;

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
            // Check if the other instance has the same song playing
            if (Instance.musicSource.clip != musicSource.clip)
            {
                // Stop the other instance's music
                Instance.StopMusic();

                // Play the music clip of the new instance
                Instance.PlayMusic(0);
            }

            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Play a random music clip
        var randomIndex = musicClips.Length > 1
            ? UnityEngine.Random.Range(0, musicClips.Length)
            : 0;

        PlayMusic(randomIndex);
    }

    // Method to play sound effects
    public void PlayOneShot(Sound sound)
    {
        // Return if the sound is null
        if (sound == null)
        {
            Debug.LogWarning("SoundManager: Invalid sound.");
            return;
        }

        // Return if the sound clip is null
        if (sound.Clip == null)
        {
            Debug.LogWarning("SoundManager: Invalid sound clip.");
            return;
        }

        // Play the sound effect
        sfxSource.PlayOneShot(sound.Clip, sound.Volume);
    }

    // Play music by index
    public void PlayMusic(int index)
    {
        if (index < 0 || index >= musicClips.Length)
        {
            Debug.LogWarning("SoundManager: Invalid music index.");
            return;
        }

        musicSource.clip = musicClips[index].Clip;
        musicSource.volume = musicClips[index].Volume;

        musicSource.Play();
    }

    // Stop the currently playing music
    public void StopMusic()
    {
        musicSource.Stop();
    }
}