using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class InputManager : MonoBehaviour, IDebugManaged
{
    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }
    
    public static InputManager Instance { get; private set; }
    public PlayerControls PlayerControls { get; private set; }

    public event Action<Vector2, SwipeDirection> onSwipe;

    #region Swipe Detection

    private Vector2 _startTouchPosition;

    [SerializeField] private float swipeThreshold = 100f;

    private Vector2 _currentTouchPosition;

    private bool _isSwiping;

    private Vector2 _swipe;

    #endregion

    #region Unity Functions

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


    // Update is called once per frame
    void Update()
    {
    }

    #endregion

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
        // Resolution-based?

        // Set the swipe vector
        _swipe = difference;

        // Call an event to notify other classes that a swipe has been detected
        onSwipe?.Invoke(_swipe, DetermineDirection(_swipe));

        // TODO: When registering swipes, use dot product to determine which direction a swipe is in
    }

    #endregion

    public string GetDebugText()
    {
        return $"Is Swiping: {_isSwiping}\n" +
               $"Touch Position: {_currentTouchPosition}\n" +
               $"Swipe: {_swipe}\n";
    }

    private static SwipeDirection DetermineDirection(Vector2 obj)
    {
        var up = Vector2.up;
        var down = Vector2.down;
        var left = Vector2.left;
        var right = Vector2.right;
        var upLeft = (Vector2.up + Vector2.left).normalized;
        var upRight = (Vector2.up + Vector2.right).normalized;
        var downLeft = (Vector2.down + Vector2.left).normalized;
        var downRight = (Vector2.down + Vector2.right).normalized;

        var upDot = Vector2.Dot(obj.normalized, up);
        var downDot = Vector2.Dot(obj.normalized, down);
        var leftDot = Vector2.Dot(obj.normalized, left);   
        var rightDot = Vector2.Dot(obj.normalized, right);
        var upLeftDot = Vector2.Dot(obj.normalized, upLeft);
        var upRightDot = Vector2.Dot(obj.normalized, upRight);
        var downLeftDot = Vector2.Dot(obj.normalized, downLeft);
        var downRightDot = Vector2.Dot(obj.normalized, downRight);
        
        // Add all the dots to a sorted list
        var dots = new SortedList<float, SwipeDirection>
        {
            {upDot, SwipeDirection.Up},
            {downDot, SwipeDirection.Down},
            {leftDot, SwipeDirection.Left},
            {rightDot, SwipeDirection.Right},
            {upLeftDot, SwipeDirection.UpLeft},
            {upRightDot, SwipeDirection.UpRight},
            {downLeftDot, SwipeDirection.DownLeft},
            {downRightDot, SwipeDirection.DownRight}
        };
        
        // Return the largest dot
        return dots[dots.Keys[dots.Count - 1]];
    }
}