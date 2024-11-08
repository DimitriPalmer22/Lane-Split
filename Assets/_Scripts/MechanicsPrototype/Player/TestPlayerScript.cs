using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TestPlayerScript : MonoBehaviour, IDebugManaged
{
    public static TestPlayerScript Instance { get; private set; }

    #region Serialized Fields

    [Header("References")] [SerializeField]
    private Transform nearMissPosition;

    [SerializeField] private Transform[] wheels;
    [SerializeField] private CarRampHandler carRampHandler;

    [Header("Boost")] [SerializeField] private float maxBoost = 10;
    [SerializeField] [Min(0)] private float boostRechargeDuration = 10;
    [SerializeField] [Min(0)] private float boostDepleteDuration = 4;
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] [Min(0)] private float boostLaunchForce = 100;

    [Header("Car Stats")] [SerializeField] [Min(0)]
    private float startingMoveSpeed = 8;

    [SerializeField] private CountdownTimer laneChangeTime;
    [SerializeField] [Min(1)] private int maxHealth = 1;

    #endregion

    #region Private Fields

    private int _lane;

    private bool _isAlive = true;

    private float _currentBoost;

    private bool _isBoosting;

    private float _currentMoveSpeed;

    private int _oldLane;

    private readonly HashSet<ObstacleScript> _currentNearMissedObstacles = new();

    #endregion

    #region Events

    public event Action<TestPlayerScript> OnBoostReady;

    public event Action<TestPlayerScript> OnBoostStart;

    public event Action<TestPlayerScript> OnBoostEnd;

    public event Action OnRampStart;

    public event Action<TestPlayerScript, ObstacleScript> OnNearMiss;

    public event Action<TestPlayerScript> OnLaneChangeStart;

    public event Action<TestPlayerScript> OnLaneChangeEnd;

    public event Action<TestPlayerScript> OnCrash;

    #endregion

    #region Getters

    public int Lane => _lane;
    public bool IsAlive => _isAlive;

    public bool IsBoosting => _isBoosting;

    public float BoostPercentage => Mathf.Clamp01(_currentBoost / maxBoost);

    public float BoostMultiplier => _isBoosting ? boostMultiplier : 1;

    private bool IsVulnerable => !_isBoosting;

    public float CurrentMoveSpeed => _currentMoveSpeed * BoostMultiplier;

    private bool IsChangingLanes => laneChangeTime.IsTicking && laneChangeTime.IsActive;

    private float BoostRechargeRate => maxBoost / boostRechargeDuration;
    private float BoostDepleteRate => maxBoost / boostDepleteDuration;

    #endregion

    private void Awake()
    {
        // Set the instance to this
        Instance = this;

        // Set the current move speed to the starting move speed
        _currentMoveSpeed = startingMoveSpeed;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize the input
        InputManager.Instance.OnSwipe += MoveOnSwipe;
        InputManager.Instance.PlayerControls.Gameplay.Boost.performed += OnBoostPerformed;
        InputManager.Instance.OnSwipe += BoostOnSwipe;

        // Subscribe to the OnNearMiss event
        OnNearMiss += LogNearMiss;
        OnNearMiss += NearMissBoostAdd;

        // Add this object to the debug manager
        DebugManager.Instance.AddDebugItem(this);

        // Set the player to the far left lane
        _lane = TestLevelManager.Instance.LaneCount / 2;
        _oldLane = _lane;
        SetLanePosition(true);

        // Set up the lane change timer
        laneChangeTime.OnTimerEnd += () =>
        {
            // Invoke the OnLaneChangeEnd event
            OnLaneChangeEnd?.Invoke(this);

            // Disable the lane change timer
            laneChangeTime.SetActive(false);
        };

        // Clear the near missed obstacles on lane change end
        OnLaneChangeEnd += _ => _currentNearMissedObstacles.Clear();
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

    #region Update Methods

    // Update is called once per frame
    private void Update()
    {
        // Update the lane change timer
        laneChangeTime.Update(Time.deltaTime);

        // Move the player
        MovePlayer();

        //RotateWheels
        RotateWheels();

        // Update the position of the player
        SetLanePosition();

        // Update near miss
        UpdateNearMiss();

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

    // Rotate the wheels
    private void RotateWheels()
    {
        // Rotate the wheel in wheels array
        foreach (var wheel in wheels)
        {
            // rotate the wheel based on speed
            wheel.Rotate(Vector3.right, CurrentMoveSpeed * 360 * Time.deltaTime);
        }
    }

    private void SetLanePosition(bool force = false)
    {
        // Return if the player is dead
        if (!_isAlive)
            return;

        if (force)
        {
            // Set the x position of the player based on the lane
            transform.position = TestLevelManager.Instance.GetLanePosition(_lane) +
                                 new Vector3(0, transform.position.y, transform.position.z);

            return;
        }

        // Return if the player is NOT currently changing lanes
        if (!IsChangingLanes)
            return;

        var oldLanePosition = TestLevelManager.Instance.GetLanePosition(_oldLane);
        var newLanePosition = TestLevelManager.Instance.GetLanePosition(_lane);

        // Set the x position of the player based on the lane
        transform.position = Vector3.Lerp(oldLanePosition, newLanePosition, laneChangeTime.Percentage) +
                             new Vector3(0, transform.position.y, transform.position.z);
    }

    private void UpdateBoost()
    {
        // Return if the player is dead
        if (!_isAlive)
            return;

        // Decrease the boost
        if (_isBoosting)
            AddBoost(-BoostDepleteRate * Time.deltaTime);

        // Add boost
        else AddBoost(BoostRechargeRate * Time.deltaTime);


        // If the boost is empty, set the boost flag to false
        if (_currentBoost <= 0)
        {
            // If the player was boosting, invoke the OnBoostEnd event
            if (_isBoosting)
                OnBoostEnd?.Invoke(this);

            _isBoosting = false;
        }
    }

    private void UpdateNearMiss()
    {
        // Return if the player is not lane changing
        if (!IsChangingLanes)
            return;

        // Return if the player is currently boosting
        if (_isBoosting)
            return;

        // Get all Obstacle scripts
        var allObstacles = TestLevelManager.Instance.LevelGenerator.SpawnedLanes
            .SelectMany(n => n.Value)
            .Where(n => n.HasObstacle)
            .Select(n => n.Obstacle);

        // Get all obstacles within near miss distance
        var validObstacles = allObstacles
            .Where(
                n => Vector3.Distance(n.NearMissPosition.position, nearMissPosition.position) <=
                     TestLevelManager.Instance.NearMissSize
            );

        // Invoke the OnNearMiss event
        foreach (var obstacle in validObstacles)
        {
            // Skip if the obstacle is in the previous lane
            if (obstacle.TestLaneScript.LaneNumber != _oldLane)
                continue;

            // Skip if the obstacle has already been near missed
            if (_currentNearMissedObstacles.Contains(obstacle))
                continue;

            // Add the obstacle to the near missed obstacles
            _currentNearMissedObstacles.Add(obstacle);

            // Invoke the OnNearMiss event
            OnNearMiss?.Invoke(this, obstacle);
        }
    }

    #endregion

    // Trigger the ramp start event when the player enters the ramp
    public void TriggerRampStart()
    {
        OnRampStart?.Invoke();
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

        // Return if the player is currently changing lanes
        if (IsChangingLanes)
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

    #endregion

    private void ChangeLanes(int modifier)
    {
        var oldLane = _lane;

        // Ensure the lane is within the bounds
        _lane = Mathf.Clamp(_lane + modifier, 0, TestLevelManager.Instance.LaneCount - 1);

        // return if there was no change
        if (oldLane == _lane)
            return;

        // Set the old lane to the current lane
        _oldLane = oldLane;

        // Reset the lane change timer
        laneChangeTime.Reset();
        laneChangeTime.SetActive(true);
        OnLaneChangeStart?.Invoke(this);
    }

    private void KillPlayer()
    {
        // Set the player to dead
        _isAlive = false;

        OnCrash?.Invoke(this);
    }

    private void AddBoost(float amount)
    {
        var wasBoostReady = _currentBoost >= maxBoost;

        _currentBoost = Mathf.Clamp(_currentBoost + amount, 0, maxBoost);

        if (_currentBoost >= maxBoost && !wasBoostReady)
            OnBoostReady?.Invoke(this);
    }

    public void MultiplyMoveSpeed(float mult)
    {
        _currentMoveSpeed *= mult;
    }

    public void AddMoveSpeed(float amt)
    {
        _currentMoveSpeed += amt;
    }

    public string GetDebugText()
    {
        return $"Player Alive?: {_isAlive}\n" +
               $"Boost: {_currentBoost} / {maxBoost} ({BoostPercentage})\n" +
               $"Is Boosting: {_isBoosting}\n" +
               $"Lane Change Time: {laneChangeTime.TimeLeft:0.00} / {laneChangeTime.MaxTime:0.00}\n";
    }

    private void OnTriggerEnter(Collider other)
    {
        // Return if the player is dead
        if (!_isAlive)
            return;

        // Check if the player has collided with an obstacle, if so kill the player
        if (!other.CompareTag("Obstacle"))
            return;

        Debug.Log($"Player collided with: {other.name} ({other.tag})");

        // Kill the player if they are vulnerable
        if (IsVulnerable)
            KillPlayer();

        // Otherwise, launch the obstacle at a random angle
        else if (_isBoosting)
        {
            // Get the rigidbody of the obstacle
            var obstacle = other.GetComponent<ObstacleScript>();
            var rb = obstacle.Rigidbody;

            // Randomize a float between 0 and 1
            var random = UnityEngine.Random.Range(-1f, 1f);

            // Create a new Vector3 for launch Angle
            var launchAngle = (Vector3.up + transform.forward + (Vector3.right * random)).normalized;
            // var launchAngle = Vector3.up;

            // Set the rb to use gravity
            rb.useGravity = true;

            // Set the rb to be non-kinematic
            rb.isKinematic = false;

            // Add a force to the obstacle
            rb.AddForce(launchAngle * boostLaunchForce, ForceMode.Impulse);
        }
    }


    #region Event Functions

    private void LogNearMiss(TestPlayerScript player, ObstacleScript obstacle)
    {
        Debug.Log($"{player.name} near Missed: {obstacle.name}");
    }

    private void NearMissBoostAdd(TestPlayerScript player, ObstacleScript obstacle)
    {
        AddBoost(maxBoost / 16);
    }

    #endregion
}