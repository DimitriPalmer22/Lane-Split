using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;



public class TestPlayerScript : MonoBehaviour, IDebugManaged
{
    private int _lane;
    
    private bool _isAlive = true;

    [SerializeField] private float maxBoost = 10;
    private float _currentBoost;

    private Rigidbody rb;
    private bool isDrifting;
   
    public float speed = 10.0f;
    private float driftRotationSpeed = 100.0f; // Increase rotation speed for a tighter drift
    private float driftForceMultiplier = 1.5f; // Extra force to simulate tighter control

    private float minRotation = 0f;
    private float maxRotation = 360f;

    //reference to playercontrols for drifting test
    private PlayerControls inputActions; // Reference to input actions



    #region Getters

    public int Lane => _lane;
    public bool IsAlive => _isAlive;

    #endregion

    // Start is called before the first frame update
    private void Awake()
    {
        // Initialize input actions
        inputActions = new PlayerControls();
        inputActions.Gameplay.drift.performed += ctx => StartDrift(); // Subscribe to drift action
        inputActions.Gameplay.drift.canceled += ctx => StopDrift();   // Subscribe to stop drift action
    }
    void Start()
    {
        // Initialize the input
        InputManager.Instance.OnSwipe += MoveOnSwipe;
        InputManager.Instance.PlayerControls.Gameplay.Boost.performed += OnBoostPerformed;

        // Add this object to the debug manager
        DebugManager.Instance.AddDebugItem(this);
        // Get the rigidbody component
        rb = GetComponent<Rigidbody>();


        // Set the player to the far left lane
        _lane = 0;
        SetLanePosition();
    }



    private void OnDestroy()
    {
        // Remove this object from the debug manager
        InputManager.Instance.OnSwipe -= MoveOnSwipe;
        InputManager.Instance.PlayerControls.Gameplay.Boost.performed -= OnBoostPerformed;

        // Remove this object from the debug manager
        DebugManager.Instance.RemoveDebugItem(this);
    }


    // Update is called once per frame
    void Update()
    {
        // Move the player
        MovePlayer();

        // Update the position of the player
        SetLanePosition();
    }

    private void MovePlayer()
    {
        // Return if the player is dead
        if (!_isAlive)
            return;

        var moveAmount = TestLevelManager.Instance.MoveSpeed * Time.deltaTime;

        // Move the player forward
        transform.position += transform.forward * moveAmount;

        // Add the distance travelled to the level generator
        TestLevelManager.Instance.LevelGenerator.AddDistanceTravelled(moveAmount);
    }


    #region Input Functions

    private void MoveOnSwipe(Vector2 swipe, InputManager.SwipeDirection direction)
    {
        // Return if the player is dead
        if (!_isAlive)
            return;
        
        // Boost the player if they swipe up
        if (direction == InputManager.SwipeDirection.Up)
        {
            Boost();
            return;
        }

        // Update the lane based on the swipe direction
        var modifier = direction switch
        {
            InputManager.SwipeDirection.Left => -1,
            InputManager.SwipeDirection.Right => 1,
            _ => 0
        };

        ChangeLanes(modifier);
    }

    private void OnBoostPerformed(InputAction.CallbackContext context)
    {
        Boost();
    }

    private void Boost()
    {
        Debug.Log("Boosting");
    }

    private void ChangeLanes(int modifier)
    {
        int oldLane = _lane;

        // Ensure the lane is within the bounds
        _lane = Mathf.Clamp(_lane + modifier, 0, TestLevelManager.Instance.LaneCount - 1);

        // return if there was no change
        if (oldLane == _lane)
            return;

        // Update the position of the player
        SetLanePosition();

        // Near miss code
        // Get all Obstacle scripts
        var allObstacles = TestLevelManager.Instance.LevelGenerator.SpawnedLanes
            .SelectMany(n => n.Value)
            .Where(n => n.HasObstacle)
            .Select(n => n.Obstacle);

        // Get all obstacles within near miss distance
        var validObstacles = allObstacles
        .Where(
            n => Vector3.Distance(n.transform.position, transform.position) <= TestLevelManager.Instance.NearMissSize
        );
               // .Where(n => n.TestLaneScript.LaneNumber == _lane || n.TestLaneScript.LaneNumber == oldLane)
        foreach (var obstacle in validObstacles)
            Debug.Log($"Near Missed: {obstacle} {obstacle.TestLaneScript.LaneNumber}");
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        // Return if the player is dead
        if (!_isAlive)
            return;

        // Check if the player has collided with an obstacle, if so kill the player
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log($"Player collided with: {other.name} ({other.tag})");
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        Debug.Log("Killed Player!");
        
        // Set the player to dead
        _isAlive = false;
    }

    private void SetLanePosition()
    {
        // Return if the player is dead
        if (!_isAlive)
            return;

        // Set the x position of the player based on the lane
        transform.position = TestLevelManager.Instance.GetLanePosition(_lane) +
                             new Vector3(0, transform.position.y, transform.position.z);
    }

    private void AddBoost(float amount)
    {
        _currentBoost = Mathf.Clamp(_currentBoost + amount, 0, maxBoost);
    }

    public string GetDebugText()
    {
        return $"Player Alive?: {_isAlive}\n" +
               $"Boost: {_currentBoost} / {maxBoost}";
    }
private void DriftController(){


    if (isDrifting)
    {
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y = Mathf.Clamp(currentRotation.y,minRotation,maxRotation);
        transform.eulerAngles = currentRotation;
        // Rotate the game object while drifting
        transform.Rotate(0, driftRotationSpeed * Time.fixedDeltaTime, 0);

        // Optionally apply additional force for drifting effects
        Vector3 driftForce = transform.right * driftForceMultiplier;
        rb.AddForce(driftForce, ForceMode.Acceleration);
    }
    }

    private void StartDrift()
    {
        if (CanDrift()) // Check if the player is in a drift zone
        {
            isDrifting = true;
        }
    }

    private void StopDrift()
    {
        isDrifting = false;
    }


    private bool CanDrift()
    {
        return true; // Placeholder
    }


}