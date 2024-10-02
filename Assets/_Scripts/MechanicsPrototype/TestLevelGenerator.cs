using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TestLevelManager))]
public class TestLevelGenerator : MonoBehaviour, IDebugManaged
{
    private const float DISTANCE_RESET = 128;

    private TestLevelManager _levelManager;

    // How far forward the generated lanes are going.
    // Used to determine if more lanes should be spawned
    private float _laneZ;

    [SerializeField] private bool hideLaneBlocks = true;

    [SerializeField] private float spawnDistance = 16;

    [SerializeField] [Range(0.25f, 1)] private float laneScaleX = 1;
    [SerializeField] [Range(0.25f, 1)] private float laneScaleZ = 1;

    [SerializeField] [Range(0, 1)] private float obstacleChance = 0.25f;

    [Header("Materials")] [SerializeField] private Material laneMaterial;
    [SerializeField] private Material obstacleMaterial;

    // A float to keep track of how far the player has travelled.
    // Used to spawn and destroy lanes
    private float _distanceTravelled;


    private Dictionary<float, HashSet<TestLaneScript>> _spawnedLanes;

    public float LaneScaleX => laneScaleX;
    public float LaneScaleZ => laneScaleZ;

    public float DistanceTravelled => _distanceTravelled;

    public Dictionary<float, HashSet<TestLaneScript>> SpawnedLanes => _spawnedLanes;


    public bool HideLaneBlocks => hideLaneBlocks && !DebugManager.Instance.IsDebug;

    private void Awake()
    {
        // Get the level manager
        _levelManager = GetComponent<TestLevelManager>();

        // Create a new dictionary to store the lanes
        _spawnedLanes = new Dictionary<float, HashSet<TestLaneScript>>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the lanes
        InitializeLanes();

        // Add this to the debug manager
        DebugManager.Instance.AddDebugItem(this);
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the distance travelled
        var moveAmount = _levelManager.MoveSpeed * Time.deltaTime;

        // // Move the lanes backwards
        // transform.position += -Vector3.forward * moveAmount;
        //
        // // Add the move amount to the distance travelled
        // _distanceTravelled += moveAmount;

        // Check if we need to spawn more lanes
        InitializeLanes();

        // Check if we need to destroy lanes
        DestroyLanes();

        // Check if we need to move the entire level
        // MoveEntireLevel();
    }

    private void InitializeLanes()
    {
        // Spawn lanes until the distance travelled is spawnDistance units ahead of the last lane
        while (_laneZ < _distanceTravelled + spawnDistance)
        {
            // Create a new set in the dictionary to store the lanes at the current z position
            _spawnedLanes[_laneZ] = new HashSet<TestLaneScript>();

            // Create a bool array to store which lanes have obstacles
            var hasObstacle = new bool[_levelManager.LaneCount];

            var startingAreaLength = 2 * _levelManager.LaneDepth;

            if (_laneZ >= startingAreaLength)
            {
                // Loop through each lane and determine if it has an obstacle
                for (var i = 0; i < _levelManager.LaneCount; i++)
                    hasObstacle[i] = UnityEngine.Random.value < obstacleChance;

                // Check to see if all lanes have an obstacle.
                // If so, remove a random one
                if (Array.TrueForAll(hasObstacle, x => x))
                    hasObstacle[UnityEngine.Random.Range(0, _levelManager.LaneCount)] = false;
            }

            // Create a cube to represent each lane in the level
            // Skip if the current laneZ is less than the depth of the lane
            // (to prevent obstacles from spawning in the start area)
            for (var laneNumber = 0; laneNumber < _levelManager.LaneCount; laneNumber++)
            {
                // Create a new cube object
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

                // Set the parent of the object
                obj.transform.SetParent(transform);

                // Add a Test Lane Script to the object
                var laneScript = obj.AddComponent<TestLaneScript>();

                // Get the collider from the object
                var objCollider = obj.GetComponent<Collider>();
                objCollider.isTrigger = true;

                // Initialize the lane script with the obstacle value
                laneScript.Initialize(laneNumber, hasObstacle[laneNumber], laneMaterial, obstacleMaterial);

                // Set the local position of the object
                obj.transform.localPosition =
                    new Vector3(_levelManager.GetLanePosition(laneNumber).x, -1f, _laneZ);

                // Add the object to the spawned lanes
                _spawnedLanes[_laneZ].Add(laneScript);
            }

            // Increase the lane z
            _laneZ += _levelManager.LaneDepth;
        }
    }

    private void DestroyLanes()
    {
        // Get the keys of the spawned lanes
        var keys = new List<float>(_spawnedLanes.Keys);

        // Loop through each key
        foreach (var key in keys)
        {
            // Check if the key is behind the player
            if (key < _distanceTravelled - spawnDistance)
            {
                // Destroy each object in the set
                foreach (var obj in _spawnedLanes[key])
                    Destroy(obj.gameObject);

                // Clear the set
                _spawnedLanes[key].Clear();

                // Remove the key from the dictionary
                _spawnedLanes.Remove(key);
            }
        }
    }

    public void AddDistanceTravelled(float distance)
    {
        _distanceTravelled += distance;
    }

    private void MoveEntireLevel()
    {
        // Return if the transform z is less than the reset distance
        if (transform.position.z > -DISTANCE_RESET)
            return;

        // Add the distance reset to the transform position
        transform.position += Vector3.forward * DISTANCE_RESET;

        // Find the largest key in the spawned lanes
        var largestKey = 0f;
        foreach (var key in _spawnedLanes.Keys)
        {
            if (key > largestKey)
                largestKey = key;
        }

        // Get the remainder of the largest key divided by the ResetDistance
        var keyRemainder = largestKey % DISTANCE_RESET;

        // Get the difference between the largest key and the remainder
        var keyDifference = largestKey - keyRemainder;

        // Move all the lanes forward
        foreach (var key in _spawnedLanes.Keys)
        {
            foreach (var obj in _spawnedLanes[key])
            {
                obj.transform.localPosition =
                    new Vector3(
                        obj.transform.localPosition.x,
                        obj.transform.localPosition.y,
                        obj.transform.localPosition.z - keyDifference
                    );
            }
        }
    }


    public string GetDebugText()
    {
        return $"Distance Travelled: {_distanceTravelled}\n";
    }

    private void OnDrawGizmos()
    {
        if (_levelManager == null)
            return;

        Gizmos.color = Color.red;

        var forwardAmt = transform.forward * _distanceTravelled;

        // Draw the point at which the lanes will be spawned
        Gizmos.DrawLine(
            forwardAmt + _levelManager.GetLanePosition(0) + new Vector3(0, 0, spawnDistance),
            forwardAmt + _levelManager.GetLanePosition(_levelManager.LaneCount - 1) + new Vector3(0, 0, spawnDistance)
        );
    }
}