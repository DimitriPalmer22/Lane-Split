using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public bool IsRunning => Player.IsAlive;

    public TestPlayerScript Player => TestPlayerScript.Instance;

    private void Awake()
    {
        // Update the instance
        Instance = this;
        // Retrieve the player car prefab and selected material from the GameManager
        GameObject playerCarPrefab = GameManager.Instance.GetPlayerCarPrefab();
        Material selectedMaterial = GameManager.Instance.GetVehicleMaterials();

        if (playerCarPrefab != null)
        {
            // Instantiate the player car
            GameObject playerCar = Instantiate(playerCarPrefab);

            // Apply the selected material
            if (selectedMaterial != null)
            {
                // Get all renderers from the instantiated vehicle
                Renderer[] renderers = playerCar.GetComponentsInChildren<Renderer>();

                foreach (Renderer renderer in renderers)
                {
                    //skip TrailRenderer
                    if(renderer is TrailRenderer)
                    {
                        continue;
                    }
                    renderer.material = selectedMaterial;
                }
            }
            else
            {
                Debug.LogWarning("Selected material is null.");
            }
        }
        else
        {
            Debug.LogError("Player car prefab is null.");
        }
    }
      
        
        // Instantiate the player prefab from the game manager
       // Instantiate(GameManager.Instance.PlayerCarPrefab);

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