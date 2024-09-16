using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class InputManager : MonoBehaviour, IDebugManaged
{
    public static InputManager Instance { get; private set; }
    public PlayerControls PlayerControls { get; private set; }

    #region Swipe Detection

    private Vector2 _startTouchPosition;

    [SerializeField] private float swipeThreshold = 100f;

    private Vector2 _currentTouchPosition;

    private bool _isSwiping;

    private Vector2 _swipe;

    #endregion

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
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize the input
        InitializeInput();

        // Add this item to the DebugManager
        DebugManager.Instance.AddDebugItem(this);
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

    #region Input Functions

    private void InitializeInput()
    {
        PlayerControls.Gameplay.Press.started += StartSwipeDetection;
        PlayerControls.Gameplay.Press.canceled += EndSwipeDetection;
    }

    private Vector2 UpdateCurrentTouchPosition()
    {
        _currentTouchPosition = PlayerControls.Gameplay.Position.ReadValue<Vector2>();
        return _currentTouchPosition;
    }

    private void StartSwipeDetection(InputAction.CallbackContext context)
    {
        // Store the start touch position
        _startTouchPosition = UpdateCurrentTouchPosition();

        // Set the isSwiping flag to true
        _isSwiping = true;
    }

    private void EndSwipeDetection(InputAction.CallbackContext context)
    {
        // Store the end touch position
        var endTouchPosition = UpdateCurrentTouchPosition();

        // Reset the isSwiping flag
        _isSwiping = false;

        // Calculate the difference between the start and end touch positions
        var difference = endTouchPosition - _startTouchPosition;

        var normalizedDifference = difference.normalized;
        var differenceDistance = difference.magnitude;

        // TODO: Implement a system for making some swipes invalid

        // Set the swipe vector
        _swipe = difference;

        // TODO: Call an event to notify other classes that a swipe has been detected

        // TODO: When registering swipes, use dot product to determine which direction a swipe is in
    }

    #endregion


    // Update is called once per frame
    void Update()
    {
    }

    public string GetDebugText()
    {
        return $"Is Swiping: {_isSwiping}\n" +
               $"Touch Position: {_currentTouchPosition}\n" +
               $"Swipe: {_swipe}\n";
    }
}