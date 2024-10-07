using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraSpeedRumble : MonoBehaviour, IDebugManaged
{
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _noise;

    [SerializeField] private float maxShake = 2;
    [SerializeField] private float maxSpeed = 200;
    [SerializeField] private float minSpeed = 80;

    [SerializeField] private AnimationCurve shakeCurve;

    private void Awake()
    {
        // Get the virtual camera
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();

        // Get the noise settings
        _noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Add this to the debug manager
        DebugManager.Instance.AddDebugItem(this);
    }

    private void OnDestroy()
    {
        // Remove this from the debug manager
        DebugManager.Instance.RemoveDebugItem(this);
    }

    // Update is called once per frame
    void Update()
    {
        // Update the shake settings
        SetShake();
    }

    private void SetShake()
    {
        // Get the current speed
        var speed = LevelManager.Instance.Player.CurrentMoveSpeed;

        var minMaxDifference = maxSpeed - minSpeed;
        var relativeSpeed = speed - minSpeed;

        // Calculate the shake amount
        var shakeAmount = shakeCurve.Evaluate(relativeSpeed / minMaxDifference) * maxShake;

        // Set the noise settings
        _noise.m_AmplitudeGain = shakeAmount;
    }

    public string GetDebugText()
    {
        return $"Camera Shake: {_noise.m_AmplitudeGain}\n";
    }
}