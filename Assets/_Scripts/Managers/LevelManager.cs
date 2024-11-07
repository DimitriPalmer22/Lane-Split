using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public bool IsRunning => player.IsAlive;

    [SerializeField] private TestPlayerScript player;

    public TestPlayerScript Player => player;

    private void Awake()
    {
        // Update the instance
        Instance = this;
    }

    private void Start()
    {
        // Initialize the input
        InitializeInput();
    }

    private void InitializeInput()
    {
        // Initialize the input
        InputManager.Instance.PlayerControls.Gameplay.Restart.canceled += RestartLevel;
    }

    // Update is called once per frame
    void Update()
    {
        SetTimeScale();
    }

    private void SetTimeScale()
    {
        // Stop the game if the player is dead
        Time.timeScale = IsRunning ? 1 : 0;
    }


    private void RestartLevel(InputAction.CallbackContext obj)
    {
        // Skip if the game is running
        if (IsRunning)
            return;

        // Load the scene again
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}