using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerScript : MonoBehaviour, IDebugManaged
{
    [SerializeField] private float moveDistance = 10f;

    private String debugText = "";
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize the input
        InputManager.Instance.onSwipe += UpdateDebugTextOnSwipe;
        InputManager.Instance.onSwipe += MoveOnSwipe;
        
        // Add this object to the debug manager
        DebugManager.Instance.AddDebugItem(this);
    }

    private void MoveOnSwipe(Vector2 swipe, InputManager.SwipeDirection direction)
    {
        switch (direction)
        {
            case InputManager.SwipeDirection.Left:
                transform.position += -transform.right * moveDistance;
                break;

            case InputManager.SwipeDirection.Right:
                transform.position += transform.right * moveDistance;
                break;
            
            case InputManager.SwipeDirection.Up:
                transform.position += transform.forward * moveDistance;
                break;
            
            case InputManager.SwipeDirection.Down:
                transform.position += -transform.forward * moveDistance;
                break;
                
        }
    }

    private void OnDestroy()
    {
        // Remove this object from the debug manager
        InputManager.Instance.onSwipe -= UpdateDebugTextOnSwipe;
        InputManager.Instance.onSwipe -= MoveOnSwipe;
        
        // Remove this object from the debug manager
        DebugManager.Instance.RemoveDebugItem(this);
    }


    // Update is called once per frame
    void Update()
    {
    }


    #region Input Functions

    private void UpdateDebugTextOnSwipe(Vector2 obj, InputManager.SwipeDirection direction)
    {
        
        var up = Vector2.up;
        var down = Vector2.down;
        var left = Vector2.left;
        var right = Vector2.right;
        var upLeft = (Vector2.up + Vector2.left).normalized;
        var upRight = (Vector2.up + Vector2.right).normalized;
        var downLeft = (Vector2.down + Vector2.left).normalized;
        var downRight = (Vector2.down + Vector2.right).normalized;

        var upDot = Vector2.Dot(obj.normalized, up);
        var downDot = Vector2.Dot(obj.normalized, down);
        var leftDot = Vector2.Dot(obj.normalized, left);   
        var rightDot = Vector2.Dot(obj.normalized, right);
        var upLeftDot = Vector2.Dot(obj.normalized, upLeft);
        var upRightDot = Vector2.Dot(obj.normalized, upRight);
        var downLeftDot = Vector2.Dot(obj.normalized, downLeft);
        var downRightDot = Vector2.Dot(obj.normalized, downRight);
        
        debugText = $"Player Swipe Dots:\n" +
                      $"Up: {upDot}\n" +
                      $"Down: {downDot}\n" +
                      $"Left: {leftDot}\n" +
                      $"Right: {rightDot}\n" +
                      $"UpLeft: {upLeftDot}\n" +
                      $"UpRight: {upRightDot}\n" +
                      $"DownLeft: {downLeftDot}\n" +
                      $"DownRight: {downRightDot}\n" +
                      $"ACTUAL Direction: {direction}\n";
    }

    #endregion

    public string GetDebugText()
    {
        return debugText;
    }
}