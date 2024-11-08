using System;
using UnityEngine;

public class TestLaneScript : MonoBehaviour
{
    private int _laneNumber;
    private bool _hasObstacle;
    private ObstacleScript _obstacle;

    private Renderer _renderer;

    public int LaneNumber => _laneNumber;
    public bool HasObstacle => _hasObstacle;
    public ObstacleScript Obstacle => _obstacle;

    private void Awake()
    {
        // Get the renderer of the lane
        _renderer = GetComponent<Renderer>();

        // Resize();
    }

    private void Start()
    {
    }

    private void Update()
    {
        // // Resize the lane
        // Resize();

        // Update the visibility of the lane
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        // Set the visibility of the lane
        _renderer.enabled = !TestLevelManager.Instance.LevelGenerator.HideLaneBlocks;
    }


    public void Initialize(
        int laneNumber, bool obstacle, GameObject obstaclePrefab
    )
    {
        // Set the lane number
        _laneNumber = laneNumber;

        // Initialize the obstacle if there is one
        _hasObstacle = obstacle;
        if (obstacle)
            InitializeObstacle(obstaclePrefab);
    }

    private void InitializeObstacle(GameObject obstaclePrefab)
    {
        // Create the obstacle
        var obstacle = Instantiate(obstaclePrefab, transform, true);
        obstacle.name += "Obstacle";

        obstacle.transform.localPosition = new Vector3(0, 1, 0);

        // Get the obstacle script
        _obstacle = obstacle.GetComponent<ObstacleScript>();

        // Initialize the obstacle
        _obstacle.Initialize(this);
    }

    private void Resize()
    {
        // Resize the lane to the correct size
        var scaleX = TestLevelManager.Instance.LaneWidth * TestLevelManager.Instance.LevelGenerator.LaneScaleX;
        var scaleZ = TestLevelManager.Instance.LaneDepth * TestLevelManager.Instance.LevelGenerator.LaneScaleZ;

        transform.localScale = new Vector3(
            scaleX,
            transform.localScale.y,
            scaleZ
        );

        // Resize the obstacle
        ResizeObstacle();
    }

    private void ResizeObstacle()
    {
        // Resize the obstacle to the correct size
        if (!_hasObstacle)
            return;

        // Resize the obstacle if there is no parent
        if (transform.parent == null)
        {
            transform.localScale = Vector3.one;
            return;
        }

        // Resize the obstacle to the correct size based on the parent scale
        var parentScale = transform.lossyScale;
        _obstacle.transform.localScale = new Vector3(
            1 / parentScale.x,
            1 / parentScale.y,
            1 / parentScale.z
        );
    }
}