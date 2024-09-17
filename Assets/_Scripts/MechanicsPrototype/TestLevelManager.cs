using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelManager : MonoBehaviour
{
    public static TestLevelManager Instance { get; private set; }

    [SerializeField] private TestPlayerScript player;

    [Header("Lanes")] [SerializeField] private float laneWidth;
    [SerializeField] private int laneCount;

    public float LaneWidth => laneWidth;

    public int LaneCount => laneCount;

    private Vector3 LeftLanePosition => new(-laneCount / 2f * laneWidth + laneWidth / 2, 0, 0);

    private void OnEnable()
    {
        // Force the instance to this
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (var i = -laneCount / 2; i < (laneCount - laneCount / 2); i++)
        {
            var position = new Vector3(i * laneWidth + laneWidth / 2, 0, 0);
            Gizmos.DrawWireCube(position, new Vector3(laneWidth, 0.1f, 5));
        }
    }

    public Vector3 GetLanePosition(int lane)
    {
        // Ensure the lane is within the bounds
        lane = Mathf.Clamp(lane, 0, laneCount - 1);

        // Return the new position
        return LeftLanePosition + new Vector3(lane * laneWidth, 0, 0);
    }
}