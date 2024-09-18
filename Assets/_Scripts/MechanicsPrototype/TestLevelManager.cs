using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelManager : MonoBehaviour, IDebugManaged
{
    public static TestLevelManager Instance { get; private set; }

    [SerializeField] private TestPlayerScript player;

    [SerializeField] private float moveSpeed = 4;

    [SerializeField] private float speedIncreaseMultiplier = 1.5f;

    [SerializeField] private float speedIncreaseAmount = 4;

    [Tooltip("How long the level should takes in seconds.")] [SerializeField]
    private float levelLength = 10;

    private float _currentLevelTimer;

    private float _totalTime;

    [Header("Lanes")] [SerializeField] private float laneWidth;
    [SerializeField] private int laneCount;

    /// <summary>
    /// How long each lane is block is on the z-axis. 
    /// </summary>
    [SerializeField] private float laneDepth = 4;

    private TestLevelGenerator _levelGenerator;

    #region Getters

    public float MoveSpeed => moveSpeed;

    public float LaneWidth => laneWidth;
    public float LaneDepth => laneDepth;
    public int LaneCount => laneCount;

    private Vector3 LeftLanePosition => new(-laneCount / 2f * laneWidth + laneWidth / 2, 0, 0);

    public TestLevelGenerator LevelGenerator => _levelGenerator;

    #endregion

    private void OnEnable()
    {
        // Force the instance to this
        Instance = this;
    }

    private void Awake()
    {
        // Get the level generator
        _levelGenerator = GetComponent<TestLevelGenerator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Add this to the debug manager
        DebugManager.Instance.AddDebugItem(this);

        // Set the current level timer to the level length
        _currentLevelTimer = levelLength;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the total time
        _totalTime += Time.deltaTime;

        // Update the level timer
        _currentLevelTimer -= Time.deltaTime;

        // Check if the level timer has run out
        while (_currentLevelTimer <= 0)
            LevelUp();
    }

    private void LevelUp()
    {
        // Reset the level timer
        _currentLevelTimer += levelLength;

        // Increase the move speed
        moveSpeed *= speedIncreaseMultiplier;
        
        // Add the speed increase amount
        moveSpeed += speedIncreaseAmount;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (var i = -laneCount / 2; i < (laneCount - laneCount / 2); i++)
        {
            var position = new Vector3(i * laneWidth + laneWidth / 2, 0, 0);
            Gizmos.DrawWireCube(position, new Vector3(laneWidth, 0.1f, laneDepth));
        }
    }

    public Vector3 GetLanePosition(int lane)
    {
        // Ensure the lane is within the bounds
        lane = Mathf.Clamp(lane, 0, laneCount - 1);

        // Return the new position
        return LeftLanePosition + new Vector3(lane * laneWidth, 0, 0);
    }

    public string GetDebugText()
    {
        return $"Time: {_totalTime:0.00} -> {_currentLevelTimer:0.00}\n" +
               $"Speed: {moveSpeed}\n" +
               $"Player Lane: {player.Lane}\n";
    }
}