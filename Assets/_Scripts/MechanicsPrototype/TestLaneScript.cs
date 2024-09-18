using System;
using UnityEngine;

public class TestLaneScript : MonoBehaviour
{
    private bool _hasObstacle;
    private GameObject _obstacle;

    private void Awake()
    {
        Resize();
    }

    private void Start()
    {
    }

    private void Update()
    {
        Resize();
    }

    public void Initialize(bool obstacle, Material laneMaterial, Material obstacleMaterial)
    {
        // Set the material of the lane
        GetComponent<Renderer>().material = laneMaterial;

        // Initialize the obstacle if there is one
        _hasObstacle = obstacle;
        if (obstacle)
            InitializeObstacle(obstacleMaterial);
    }

    private void InitializeObstacle(Material obstacleMaterial)
    {
        // Create the obstacle
        _obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _obstacle.name = "Obstacle";
        _obstacle.tag = "Obstacle";

        _obstacle.transform.SetParent(transform);
        _obstacle.transform.localPosition = new Vector3(0, 1, 0);

        // Set the material of the obstacle
        _obstacle.GetComponent<Renderer>().material = obstacleMaterial;
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