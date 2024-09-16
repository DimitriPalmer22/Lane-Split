using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public PlayerControls PlayerControls { get; private set; }

    private void Awake()
    {
        // Set the instance to this object if it is null
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Don't destroy this object when loading a new scene
        DontDestroyOnLoad(gameObject);

        // Create a new instance of the PlayerControls class
        PlayerControls = new PlayerControls();

        // // TODO: Delete?
        // // Enable Touch Simulation
        // TouchSimulation.Enable();
        
        Debug.Log("InputManager Awake!");
    }

    private void OnEnable()
    {
        // Enable the PlayerControls
        PlayerControls.Enable();
    }

    private void OnDisable()
    {
        // Disable the PlayerControls
        PlayerControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}