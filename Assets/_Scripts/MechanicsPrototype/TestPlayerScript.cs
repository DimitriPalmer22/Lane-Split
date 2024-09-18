using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerScript : MonoBehaviour, IDebugManaged
{
    private int _lane;
    
    private bool _isAlive = true;

    public int Lane => _lane;
    
    public bool IsAlive => _isAlive;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the input
        InputManager.Instance.OnSwipe += MoveOnSwipe;

        // Add this object to the debug manager
        DebugManager.Instance.AddDebugItem(this);

        // Set the player to the far left lane
        _lane = 0;
        SetLanePosition();
    }


    private void OnDestroy()
    {
        // Remove this object from the debug manager
        InputManager.Instance.OnSwipe -= MoveOnSwipe;

        // Remove this object from the debug manager
        DebugManager.Instance.RemoveDebugItem(this);
    }


    // Update is called once per frame
    void Update()
    {
        // Move the player
        MovePlayer();

        // Update the position of the player
        SetLanePosition();
    }

    private void MovePlayer()
    {
        // Return if the player is dead
        if (!_isAlive)
            return;

        var moveAmount = TestLevelManager.Instance.MoveSpeed * Time.deltaTime;

        // Move the player forward
        transform.position += transform.forward * moveAmount;

        // Add the distance travelled to the level generator
        TestLevelManager.Instance.LevelGenerator.AddDistanceTravelled(moveAmount);
    }


    #region Input Functions

    private void MoveOnSwipe(Vector2 swipe, InputManager.SwipeDirection direction)
    {
        // Return if the player is dead
        if (!_isAlive)
            return;
        
        // Boost the player if they swipe up
        if (direction == InputManager.SwipeDirection.Up)
        {
            Boost();
            return;
        }

        // Update the lane based on the swipe direction
        var modifier = direction switch
        {
            InputManager.SwipeDirection.Left => -1,
            InputManager.SwipeDirection.Right => 1,
            _ => 0
        };

        ChangeLanes(modifier);
    }

    private void Boost()
    {
        Debug.Log("Boosting!");
    }

    private void ChangeLanes(int modifier)
    {
        // Ensure the lane is within the bounds
        _lane = Mathf.Clamp(_lane + modifier, 0, TestLevelManager.Instance.LaneCount - 1);

        // Update the position of the player
        SetLanePosition();
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        // Return if the player is dead
        if (!_isAlive)
            return;

        // Check if the player has collided with an obstacle, if so kill the player
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log($"Player collided with: {other.name} ({other.tag})");
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        Debug.Log("Killed Player!");
        
        // Set the player to dead
        _isAlive = false;
    }

    private void SetLanePosition()
    {
        // Return if the player is dead
        if (!_isAlive)
            return;

        // Set the x position of the player based on the lane
        transform.position = TestLevelManager.Instance.GetLanePosition(_lane) +
                             new Vector3(0, transform.position.y, transform.position.z);
    }

    public string GetDebugText()
    {
        return $"Player Alive?: {_isAlive}\n";
    }
}