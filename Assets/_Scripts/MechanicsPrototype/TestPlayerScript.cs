using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerScript : MonoBehaviour, IDebugManaged
{
    private String _debugText = "";

    private int _lane;
    
    public int Lane => _lane;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the input
        InputManager.Instance.onSwipe += MoveOnSwipe;

        // Add this object to the debug manager
        DebugManager.Instance.AddDebugItem(this);

        // Set the player to the far left lane
        _lane = 0;
        SetLanePosition();
    }


    private void OnDestroy()
    {
        // Remove this object from the debug manager
        InputManager.Instance.onSwipe -= MoveOnSwipe;

        // Remove this object from the debug manager
        DebugManager.Instance.RemoveDebugItem(this);
    }


    // Update is called once per frame
    void Update()
    {
        // Update the position of the player
        SetLanePosition();
    }


    #region Input Functions

    private void MoveOnSwipe(Vector2 swipe, InputManager.SwipeDirection direction)
    {
        // Update the lane based on the swipe direction
        var modifier = direction switch
        {
            InputManager.SwipeDirection.Left => -1,
            InputManager.SwipeDirection.Right => 1,
            _ => 0
        };

        // Ensure the lane is within the bounds
        _lane = Mathf.Clamp(_lane + modifier, 0, TestLevelManager.Instance.LaneCount - 1);

        // Update the position of the player
        SetLanePosition();
    }

    #endregion

    private void SetLanePosition()
    {
        // Set the x position of the player based on the lane
        transform.position = TestLevelManager.Instance.GetLanePosition(_lane) +
                             new Vector3(0, transform.position.y, transform.position.z);
    }

    public string GetDebugText()
    {
        return _debugText;
    }
}