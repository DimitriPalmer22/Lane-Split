using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFOVHandler : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;

    [SerializeField] private float defaultFOV = 80f;

    [SerializeField] [Range(.01f, 10)] private float boostFOVMultiplier = 1.5f;

    [SerializeField] [Min(0)] private float boostFOVTransitionTime = 0.5f;

    private float _currentBoostTransition;

    private int _boostTransitionState;

    private void Awake()
    {
        // Get the Cinemachine Virtual Camera component
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.Instance.Player.OnBoostStart += OnBoostStart;
        LevelManager.Instance.Player.OnBoostEnd += OnBoostEnd;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the FOV
        SetFOV();
    }

    private void SetFOV()
    {
        var newFOV = defaultFOV;

        // If the player is boosting,
        // set the new FOV to the default FOV multiplied by the boost FOV multiplier
        if (_boostTransitionState != 0)
        {
            // Update the current boost transition
            _currentBoostTransition =
                Mathf.Clamp(
                    _currentBoostTransition + Time.deltaTime * _boostTransitionState,
                    0,
                    boostFOVTransitionTime
                );

            // Get the transition percentage
            var transitionPercentage = _currentBoostTransition / boostFOVTransitionTime;

            newFOV = Mathf.Lerp(defaultFOV, defaultFOV * boostFOVMultiplier, transitionPercentage);
        }

        // Set the new FOV
        vCam.m_Lens.FieldOfView = newFOV;

        // If the current transition state is -1 and the current boost transition is 0,
        // set the boost transition state to 0
        if (_boostTransitionState == -1 && _currentBoostTransition == 0)
            _boostTransitionState = 0;
    }


    private void OnBoostStart(TestPlayerScript obj)
    {
        // Start the boost transition
        _boostTransitionState = 1;

        // Set the current boost transition to 0
        _currentBoostTransition = 0;
    }

    private void OnBoostEnd(TestPlayerScript obj)
    {
        // Start the boost transition
        _boostTransitionState = -1;
    }
}