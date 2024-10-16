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

    [SerializeField] private float boostMultiplier = 2f;

    private float _currentBoost;

    private bool _isBoosting;

    public Transform[] wheels;

    public event Action<TestPlayerScript> OnBoostStart;

    public event Action<TestPlayerScript> OnBoostEnd;

    public event Action OnRampStart;

    //*Reference to the car ramp handler & sound manager scripts
    [SerializeField] private CarRampHandler carRampHandler;
    [SerializeField] private SoundManager soundManager;

    #region Getters

    public int Lane => _lane;
    public bool IsAlive => _isAlive;

    public bool IsBoosting => _isBoosting;

    public float BoostPercentage => Mathf.Clamp01(_currentBoost / maxBoost);

    public float BoostMultiplier => _isBoosting ? boostMultiplier : 1;

    private bool IsVulnerable => !_isBoosting;

    public float CurrentMoveSpeed => TestLevelManager.Instance.MoveSpeed * BoostMultiplier;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the input
        InputManager.Instance.OnSwipe += MoveOnSwipe;
        InputManager.Instance.PlayerControls.Gameplay.Boost.performed += OnBoostPerformed;
        InputManager.Instance.OnSwipe += BoostOnSwipe;

        // Add this object to the debug manager
        DebugManager.Instance.AddDebugItem(this);

        // Set the player to the far left lane
        _lane = 0;
        SetLanePosition();
    }

    private void OnDestroy()
    {
        // Remove this object from the debug manager
        InputManager.Instance.OnSwipe -= MoveOnSwipe;
        InputManager.Instance.PlayerControls.Gameplay.Boost.performed -= OnBoostPerformed;
        InputManager.Instance.OnSwipe -= BoostOnSwipe;

        // Remove this object from the debug manager
        DebugManager.Instance.RemoveDebugItem(this);
    }


    // Update is called once per frame
    void Update()
    {
        // Move the player
        MovePlayer();

        //RotateWheels
        RotateWheels();

        // Update the position of the player
        SetLanePosition();

        // Update boost
        UpdateBoost();
    }

    private void MovePlayer()
    {
        // Return if the player is dead
        if (!_isAlive)
            return;

        var moveAmount = CurrentMoveSpeed * Time.deltaTime;

        if (carRampHandler.IsRamping)
            Time.timeScale = 0.5f;
        else
            Time.timeScale = 1;

        //Move the player forward
        transform.position += transform.forward * moveAmount;

        // Add the distance travelled to the level generator
        TestLevelManager.Instance.LevelGenerator.AddDistanceTravelled(moveAmount);
    }

    // Trigger the ramp start event when the player enters the ramp
    public void TriggerRampStart()
    {
        OnRampStart?.Invoke();
    }

    //*Rotate the wheels
    private void RotateWheels()
    {
        // Rotate the wheel in wheels array 
        foreach (var wheel in wheels)
        {
            // rotate the wheel based on speed
            wheel.Rotate(Vector3.right, CurrentMoveSpeed * 360 * Time.deltaTime);
        }
    }


    #region Input Functions

    private void MoveOnSwipe(Vector2 swipe, InputManager.SwipeDirection direction)
    {
        // Return if the player is dead
        if (!_isAlive)
            return;

        // Disable movement while ramping
        if (carRampHandler.IsRamping)
            return;

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


    private void BoostOnSwipe(Vector2 swipe, InputManager.SwipeDirection direction)
    {
        // Boost the player if they swipe up
        if (direction != InputManager.SwipeDirection.Up)
            return;

        Boost();
    }

    private void Boost()
    {
        if (!_isAlive)
            return;

        // Disable boosting while ramping
        if (carRampHandler.IsRamping)
            return;

        // Skip if the player is already boosting
        if (_isBoosting)
            return;

        // If the boost isn't full, return
        if (BoostPercentage < 1)
            return;

        // Set the boost flag to true
        _isBoosting = true;

        // Invoke the OnBoostStart event
        OnBoostStart?.Invoke(this);
    }

    private void UpdateBoost()
    {
        // Return if the player is dead
        if (!_isAlive)
            return;

        // Return if the player isn't boosting
        // Decrease the boost
        if (_isBoosting)
            AddBoost(-1 * Time.deltaTime);

        // Add boost
        else
            AddBoost(1 * Time.deltaTime);

        // If the boost is empty, set the boost flag to false
        if (_currentBoost <= 0)
        {
            // If the player was boosting, invoke the OnBoostEnd event
            if (_isBoosting)
                OnBoostEnd?.Invoke(this);

            _isBoosting = false;
        }
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
                n => Vector3.Distance(n.transform.position, transform.position) <=
                     TestLevelManager.Instance.NearMissSize
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

            if (IsVulnerable)
                KillPlayer();

            else if (_isBoosting)
            {
                // Get the rigidbody of the obstacle
                var rb = other.GetComponent<Rigidbody>();

                // Randomize a float between 0 and 1
                var random = UnityEngine.Random.Range(-1f, 1f);

                // Create a new Vector3 for launch Angle
                // var launchAngle = Vector3.up + transform.forward + Vector3.right * random;
                var launchAngle = Vector3.up;

                // Set the rb to use gravity
                rb.useGravity = true;

                // Add a force to the obstacle
                rb.AddForce(launchAngle * 1000, ForceMode.Impulse);
            }
        }
    }

    private void KillPlayer()
    {
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
               $"Boost: {_currentBoost} / {maxBoost} ({BoostPercentage})\n" +
               $"Is Boosting: {_isBoosting}\n";
    }
}