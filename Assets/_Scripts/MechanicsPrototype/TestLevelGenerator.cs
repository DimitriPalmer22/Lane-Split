using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TestLevelManager))]
public class TestLevelGenerator : MonoBehaviour
{
    private TestLevelManager _levelManager;

    private void Awake()
    {
        // Get the level manager
        _levelManager = GetComponent<TestLevelManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeLanes();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void InitializeLanes()
    {
        for (var i = 0; i < _levelManager.LaneCount; i++)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            var laneX = _levelManager.GetLanePosition(i);
            
            obj.transform.position = laneX + new Vector3(0, -1f, 0);
        }
    }
}