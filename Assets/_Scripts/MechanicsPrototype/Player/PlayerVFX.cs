using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] private Volume volume;

    [Header("Boost Ready Chromatic Aberration")] [SerializeField] [Min(0)]
    private float maxBoostChromaticAberration = 0.5f;

    [SerializeField] private AnimationCurve boostChromaticAberrationCurve;
    [SerializeField] private CountdownTimer boostChromaticAberrationTimer;

    [Header("Boosting Vignette")] [SerializeField] [Min(0)]
    private float maxBoostVignette = 0.4f;

    [SerializeField] private CountdownTimer boostVignetteTimer = new(.5f, true, true);

    private TestPlayerScript _player;
    private ChromaticAberration _chromaticAberration;
    private Vignette _vignette;

    private void Awake()
    {
        // Get the player
        _player = GetComponent<TestPlayerScript>();

        // Initialize the VFX components
        InitializeVFXComponents();

        // Set the current boost to the max boost
        boostVignetteTimer.SetActive(true);
        boostVignetteTimer.ForceComplete();
    }

    private void InitializeVFXComponents()
    {
        // Get the chromatic aberration
        volume.profile.TryGet(out _chromaticAberration);

        // Get the vignette
        volume.profile.TryGet(out _vignette);
    }

    private void Start()
    {
        // Initialize the events
        InitializeEvents();
    }

    private void InitializeEvents()
    {
        _player.OnBoostReady += _ =>
        {
            // Start the chromatic aberration timer
            boostChromaticAberrationTimer.Reset();
            boostChromaticAberrationTimer.SetActive(true);
        };

        // Subscribe to the OnBoostStart event
        _player.OnBoostStart += _ =>
        {
            boostVignetteTimer.Reset();
            boostVignetteTimer.SetActive(true);
        };

        // Subscribe to the OnBoostEnd event
        _player.OnBoostEnd += _ =>
        {
            boostVignetteTimer.Reset();
            boostVignetteTimer.SetActive(true);
        };
    }

    private void Update()
    {
        boostVignetteTimer.Update(Time.deltaTime);
        boostChromaticAberrationTimer.Update(Time.deltaTime);

        // Update the boost chromatic aberration
        UpdateBoostChromaticAberration();

        // Update the boost vignette
        UpdateBoostVignette();
    }

    private void UpdateBoostChromaticAberration()
    {
        if (!_player.IsAlive)
            return;

        var randomChrAb = UnityEngine.Random.Range(500, 1500) / 1000f;

        var ab = boostChromaticAberrationCurve.Evaluate(boostChromaticAberrationTimer.Percentage);

        // Get the chromatic aberration volume component
        _chromaticAberration.intensity.Override(ab * randomChrAb * maxBoostChromaticAberration);
    }

    private void UpdateBoostVignette()
    {
        if (!_player.IsAlive)
            return;

        // If the player is currently boosting, apply the vignette
        if (_player.IsBoosting)
        {
            // Set the intensity of the vignette
            _vignette.intensity.Override((boostVignetteTimer.Percentage) * maxBoostVignette);
        }
        else _vignette.intensity.Override((1 - boostVignetteTimer.Percentage) * maxBoostVignette);
    }
}