using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TestLevelManager))]
public class TestLevelGenerator : MonoBehaviour, IDebugManaged
{
    private const float DISTANCE_RESET = 128;

    #region Serialized Fields

    [SerializeField] private bool hideLaneBlocks = true;

    [SerializeField] private float spawnDistance = 16;
    [SerializeField] private float lanesSpawnDistance = 200;
    [SerializeField] private float lanesDespawnDistance = 50;

    [SerializeField] [Range(0.25f, 1)] private float laneScaleX = 1;
    [SerializeField] [Range(0.25f, 1)] private float laneScaleZ = 1;

    [SerializeField] [Range(0, 1)] private float obstacleChance = 0.25f;

    [Header("Lanes")] [SerializeField] [Range(0, 1)]
    private float specialLaneChance = 0.25f;

    [SerializeField] private SpawnWeight[] normalLaneObjects;
    [SerializeField] private SpawnWeight[] specialLaneObjects;

    [Header("Vehicles")] [SerializeField] private SpawnWeight[] vehicles;

    #endregion

    #region Private Fields

    private TestLevelManager _levelManager;

    // How far forward the generated lanes are going.
    // Used to determine if more lanes should be spawned
    private float _laneZ;

    // A float to keep track of how far the player has travelled.
    // Used to spawn and destroy lanes
    private float _distanceTravelled;

    private Dictionary<float, HashSet<TestLaneScript>> _spawnedLanes;

    private LaneBoundsHelper _currentLaneObject;

    private float _spawnedLaneObjectDistance;

    private readonly Queue<LaneBoundsHelper> _despawnLaneObjects = new Queue<LaneBoundsHelper>();

    private bool _previousLaneSpecial = true;

    #endregion

    #region Getters

    public float LaneScaleX => laneScaleX;
    public float LaneScaleZ => laneScaleZ;

    public float DistanceTravelled => _distanceTravelled;

    public Dictionary<float, HashSet<TestLaneScript>> SpawnedLanes => _spawnedLanes;


    public bool HideLaneBlocks => hideLaneBlocks && !DebugManager.Instance.IsDebug;

    #endregion

    private void Awake()
    {
        // Get the level manager
        _levelManager = GetComponent<TestLevelManager>();

        // Create a new dictionary to store the lanes
        _spawnedLanes = new Dictionary<float, HashSet<TestLaneScript>>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize the lanes
        InitializeLanes();

        // Add this to the debug manager
        DebugManager.Instance.AddDebugItem(this);

        // Spawn the initial lanes
        SpawnLaneObjectBeforeStart();
        CreateLaneObjects();
    }

    // Update is called once per frame
    private void Update()
    {
        // Check if we need to spawn more lanes
        InitializeLanes();

        // Check if we need to destroy lanes
        DestroyLanes();

        // Create lane objects if necessary
        CreateLaneObjects();

        // Despawn lane objects if necessary
        DespawnLaneObjects();
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

                // Select a random vehicle from the vehicle list as an obstacle
                GameObject obstacle = null;


                // If the current lane has an obstacle, spawn it
                if (hasObstacle[laneNumber])
                {
                    // Get the total spawn weight
                    var totalWeight = vehicles.Sum(n => n.Weight);

                    // Get a random value between 0 and the total weight
                    var randomValue = UnityEngine.Random.Range(0, totalWeight);

                    // Loop through each vehicle to get the selected vehicle
                    int vehicleIndex;
                    for (vehicleIndex = 0; vehicleIndex < vehicles.Length; vehicleIndex++)
                    {
                        // Subtract the weight of the current vehicle from the random value
                        randomValue -= vehicles[vehicleIndex].Weight;

                        // If the random value is less than 0, select the current vehicle
                        if (randomValue <= 0)
                            break;
                    }

                    // Clamp the vehicle index to the length of the vehicles array
                    vehicleIndex = Mathf.Clamp(vehicleIndex, 0, vehicles.Length - 1);

                    // Set the obstacle to the selected vehicle's prefab
                    obstacle = vehicles[vehicleIndex].Prefab;

                    // Instantiate the selected vehicle as the obstacle
                    // obstacle = Instantiate(vehicles[vehicleIndex].Prefab, obj.transform);
                    // obstacle.transform.localPosition = new Vector3(0, 0.5f, 0);
                }

                // Initialize the lane script with the obstacle value
                laneScript.Initialize(laneNumber, hasObstacle[laneNumber], obstacle);

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

    private void CreateLaneObjects()
    {
        if (_distanceTravelled < _spawnedLaneObjectDistance - lanesSpawnDistance)
            return;

        // Randomly select to do a normal or special lane
        // Ensure that two special lanes are not spawned in a row
        var specialLane = UnityEngine.Random.value < specialLaneChance && !_previousLaneSpecial;

        var laneArr = specialLane ? specialLaneObjects : normalLaneObjects;

        // Get the total spawn weight
        var totalWeight = laneArr.Sum(n => n.Weight);

        // Get a random value between 0 and the total weight
        var randomValue = UnityEngine.Random.Range(0, totalWeight);

        // Loop through each vehicle to get the selected vehicle
        int laneIndex;
        for (laneIndex = 0; laneIndex < laneArr.Length; laneIndex++)
        {
            // Subtract the weight of the current vehicle from the random value
            randomValue -= laneArr[laneIndex].Weight;

            // If the random value is less than 0, select the current
            if (randomValue <= 0)
                break;
        }

        // Clamp the vehicle index to the length of the laneArr array
        laneIndex = Mathf.Clamp(laneIndex, 0, laneArr.Length - 1);

        // Set the obstacle to the selected lane
        var lane = laneArr[laneIndex];

        // Set the previous lane special flag
        _previousLaneSpecial = specialLane;

        SpawnLaneObject(lane.Prefab);
    }

    private void SpawnLaneObject(GameObject prefab)
    {
        // Spawn the lane object at the current position
        var newLaneObject = Instantiate(prefab);

        if (!newLaneObject.TryGetComponent(out LaneBoundsHelper boundsHelper))
        {
            Debug.LogError($"{newLaneObject} DOES NOT HAVE LANE BOUNDS HELPER");
            return;
        }

        // Offset the lane object based on its helper script
        newLaneObject.transform.position =
            new Vector3(0, -boundsHelper.YOffset, -boundsHelper.StartZ + _spawnedLaneObjectDistance);

        // Set the current lane object
        _currentLaneObject = boundsHelper;

        // Set the spawned lane object distance
        _spawnedLaneObjectDistance += boundsHelper.TotalLength;

        // Add the lane object to the despawn queue
        _despawnLaneObjects.Enqueue(boundsHelper);
    }

    private void SpawnLaneObjectBeforeStart()
    {
        // Get a random lane from the lane objects
        var lane = normalLaneObjects[UnityEngine.Random.Range(0, normalLaneObjects.Length)];

        // Get the lane bounds helper from the lane object
        if (!lane.Prefab.TryGetComponent(out LaneBoundsHelper boundsHelper))
        {
            Debug.LogError($"{lane} DOES NOT HAVE LANE BOUNDS HELPER");
            return;
        }

        // Offset the _spawnedLaneObjectDistance based on the lane object's length
        _spawnedLaneObjectDistance -= boundsHelper.TotalLength;

        // Spawn the lane object at the current position
        SpawnLaneObject(lane.Prefab);
    }

    private void DespawnLaneObjects()
    {
        if (_despawnLaneObjects.Count == 0)
            return;

        // Get the first lane object in the queue
        var laneObject = _despawnLaneObjects.Peek();

        // Check if the lane object is behind the player
        // Destroy the lane object
        if (laneObject.CurrentStartZ + laneObject.TotalLength < _distanceTravelled - lanesDespawnDistance)
            Destroy(_despawnLaneObjects.Dequeue().gameObject);
    }

    public string GetDebugText()
    {
        return $"Distance Travelled: {_distanceTravelled}\n" +
               $"Spawned Lane Distance: {_spawnedLaneObjectDistance}\n";
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