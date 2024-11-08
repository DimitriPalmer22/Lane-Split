using System;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField] private Sound rampSound;

    [SerializeField] private Sound laneChangeSound;
    [SerializeField] private Sound nearMissSound;

    [SerializeField] private Sound boostReadySound;
    [SerializeField] private Sound boostSound;

    [SerializeField] private Sound crashSound;

    private TestPlayerScript _player;

    private void Awake()
    {
        // Get the player script
        _player = GetComponent<TestPlayerScript>();
    }

    private void Start()
    {
        // Subscribe to the events
        _player.OnRampStart += () => PlaySound(rampSound);
        _player.OnLaneChangeStart += _ => PlaySound(laneChangeSound);
        _player.OnNearMiss += (_, _) => PlaySound(nearMissSound);
        _player.OnBoostReady += _ => PlaySound(boostReadySound);
        _player.OnBoostStart += _ => PlaySound(boostSound);
        _player.OnCrash += _ => PlaySound(crashSound);
    }

    private void PlaySound(Sound sound)
    {
        // Play the sound
        SoundManager.Instance.PlayOneShot(sound);
    }
}