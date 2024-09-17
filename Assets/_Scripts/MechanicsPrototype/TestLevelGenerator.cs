using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TestLevelManager))]
public class TestLevelGenerator : MonoBehaviour, IDebugManaged
{
    private TestLevelManager _levelManager;

    // How far forward the generated lanes are going.
    // Used to determine if more lanes should be spawned
    private float _laneZ;

    [SerializeField] private float spawnDistance = 16;

    // A float to keep track of how far the player has travelled.
    // Used to spawn and destroy lanes
    private float _distanceTravelled;

    private const float DISTANCE_RESET = 128;

    private Dictionary<float, HashSet<GameObject>> _spawnedLanes;

    private void Awake()
    {
        // Get the level manager
        _levelManager = GetComponent<TestLevelManager>();

        // Create a new dictionary to store the lanes
        _spawnedLanes = new Dictionary<float, HashSet<GameObject>>();
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

        // Move the lanes backwards
        transform.position += -Vector3.forward * moveAmount;

        // Add the move amount to the distance travelled
        _distanceTravelled += moveAmount;

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
            _spawnedLanes[_laneZ] = new HashSet<GameObject>();

            // Create a cube to represent each lane in the level
            for (var i = 0; i < _levelManager.LaneCount; i++)
            {
                // Create a new cube object
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

                // Set the parent of the object
                obj.transform.SetParent(transform);

                // Set the local position of the object
                obj.transform.localPosition =
                    new Vector3(_levelManager.GetLanePosition(i).x, -1f, _laneZ);

                // Update the scale of the object
                obj.transform.localScale =
                    new Vector3(_levelManager.LaneWidth * 7 / 8, 1, _levelManager.LaneDepth * 7 / 8);

                // Add the object to the spawned lanes
                _spawnedLanes[_laneZ].Add(obj);
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
                // Loop through each object in the set
                foreach (var obj in _spawnedLanes[key])
                {
                    // Destroy the object
                    Destroy(obj);
                }

                // Clear the set
                _spawnedLanes[key].Clear();

                // Remove the key from the dictionary
                _spawnedLanes.Remove(key);
            }
        }
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
        return $"Lane Z: {_laneZ}\n" +
               $"Distance Travelled: {_distanceTravelled}\n";
    }

    private void OnDrawGizmos()
    {
        if (_levelManager == null)
            return;

        Gizmos.color = Color.red;

        // Draw the point at which the lanes will be spawned
        Gizmos.DrawLine(
            _levelManager.GetLanePosition(0) + new Vector3(0, 0, spawnDistance),
            _levelManager.GetLanePosition(_levelManager.LaneCount - 1) + new Vector3(0, 0, spawnDistance)
        );
    }
}