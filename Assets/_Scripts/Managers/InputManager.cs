using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public event Action<Vector2, SwipeDirection> OnSwipe;

    #region Swipe Detection

    private Vector2 _startTouchPosition;

    private Vector2 _currentTouchPosition;

    private bool _isSwiping;

    private Vector2 _swipe;

    [SerializeField] [Tooltip("What percentage of the screen should the swipe be to be considered a swipe?")]
    [Range(0, 1)]
    private float swipeDetectionThreshold = 0.1f;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        // // Set the instance to this object if it is null
        // if (Instance == null)
        //     Instance = this;
        // else
        //     Destroy(gameObject);
        //
        // // Don't destroy this object when loading a new scene
        // DontDestroyOnLoad(gameObject);

        // Set the instance to this object
        Instance = this;
        
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

        PlayerControls.Gameplay.Swerve.started += OnSwerve;
    }

    private void OnSwerve(InputAction.CallbackContext context)
    {
        // Read the direction of the swerve
        var direction = context.ReadValue<float>();

        if (direction == 0)
            return;

        var tmpSwipe = Vector2.right * direction;

        SwipeDirection swipeDirection = default;

        if (direction < 0)
            swipeDirection = SwipeDirection.Left;
        else
            swipeDirection = SwipeDirection.Right;


        // Call an event to notify other classes that a swipe has been detected
        OnSwipe?.Invoke(tmpSwipe, swipeDirection);

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

        // a system for making some swipes invalid
        if (!IsValidSwipe(difference))
            return;

        // Set the swipe vector
        _swipe = difference;

        // Determine which direction the swipe is in
        var direction = DetermineDirection(_swipe);

        // Call an event to notify other classes that a swipe has been detected
        OnSwipe?.Invoke(_swipe, direction);
    }

    #endregion

    public string GetDebugText()
    {
        return $"Is Swiping: {_isSwiping}\n" +
               $"Touch Position: {_currentTouchPosition}\n" +
               $"Swipe: {_swipe}\n";
    }

    private bool IsValidSwipe(Vector2 swipe)
    {
        // Get the screen width and height
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;

        // Determine which dimension is smaller
        var smallerDimension = screenWidth < screenHeight ? screenWidth : screenHeight;

        // Calculate the swipe threshold
        var swipeThreshold = smallerDimension * swipeDetectionThreshold;

        // If the swipe distance is less than the threshold, return false
        if (swipe.magnitude < swipeThreshold)
            return false;

        return true;
    }

    private static SwipeDirection DetermineDirection(Vector2 obj)
    {
        // Make vectors for 8 directions
        var up = Vector2.up;
        var down = Vector2.down;
        var left = Vector2.left;
        var right = Vector2.right;
        var upLeft = (Vector2.up + Vector2.left).normalized;
        var upRight = (Vector2.up + Vector2.right).normalized;
        var downLeft = (Vector2.down + Vector2.left).normalized;
        var downRight = (Vector2.down + Vector2.right).normalized;

        // Create dot products from the swipe vector and the 8 directions
        var upDot = Vector2.Dot(obj.normalized, up);
        var downDot = Vector2.Dot(obj.normalized, down);
        var leftDot = Vector2.Dot(obj.normalized, left);
        var rightDot = Vector2.Dot(obj.normalized, right);
        var upLeftDot = Vector2.Dot(obj.normalized, upLeft);
        var upRightDot = Vector2.Dot(obj.normalized, upRight);
        var downLeftDot = Vector2.Dot(obj.normalized, downLeft);
        var downRightDot = Vector2.Dot(obj.normalized, downRight);

        // Add all the dots to a dictionary
        var dotsDict = new Dictionary<SwipeDirection, float>
        {
            { SwipeDirection.Up, upDot },
            { SwipeDirection.Down, downDot },
            { SwipeDirection.Left, leftDot },
            { SwipeDirection.Right, rightDot },
            { SwipeDirection.UpLeft, upLeftDot },
            { SwipeDirection.UpRight, upRightDot },
            { SwipeDirection.DownLeft, downLeftDot },
            { SwipeDirection.DownRight, downRightDot }
        };

        var largest = SwipeDirection.Up;
        foreach (var item in dotsDict.Keys)
        {
            if (dotsDict[item] > dotsDict[largest])
                largest = item;
        }

        // Return the largest dot product.
        // This is the direction the player MOST LIKELY swiped
        return largest;
    }
}