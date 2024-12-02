using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VehicleSelectManager : MonoBehaviour
{
    #region Serialized Fields

    // Array to hold vehicle prefabs and their info
    [SerializeField] private VehicleSelectionInfo[] vehiclePrefabs;

    [SerializeField] [Min(0)] private float rotationsPerSecond;

    #endregion

    #region Private Fields

    // Reference to the current vehicle instance
    private GameObject _currentVehicleInstance;

    // Array to hold all renderers of the current vehicle
    private Renderer[] _renderers;

    // Variable to store the selected vehicle index
    private int _selectedVehicleIndex;

    // The current rotation of the vehicle around the y-axis
    private float _currentRotation;

    // Current material index for the selected vehicle
    private int _currentMaterialIndex;

    // Current vehicle's materials
    private Material[] _currentVehicleMaterials;

    #endregion

    private void Start()
    {
        _currentMaterialIndex = 0;

        // Input manager to subscribe to the swipe event
        InputManager.Instance.OnSwipe += OnSwipeSelectVehicle;

        InputManager.Instance.PlayerControls.Gameplay.Boost.performed += _ => LoadSelectedVehicle();

        InstantiateSelectedVehicle();
    }

    private void OnSwipeSelectVehicle(Vector2 swipe, InputManager.SwipeDirection direction)
    {
        switch (direction)
        {
            case InputManager.SwipeDirection.Left:
                SelectPreviousVehicle();
                break;

            case InputManager.SwipeDirection.Right:
                SelectNextVehicle();
                break;

            case InputManager.SwipeDirection.Up:
                LoadSelectedVehicle();
                break;

            case InputManager.SwipeDirection.Down:
                // Change the material of the vehicle when the player swipes down
                ChangeVehicleMaterial();
                break;
        }
    }

    private void Update()
    {
        // Update the rotation of the vehicle
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        var rotationAmount = 360 * rotationsPerSecond * Time.deltaTime;
        _currentRotation = (_currentRotation + rotationAmount) % 360;

        if (_currentVehicleInstance != null)
            _currentVehicleInstance.transform.rotation = Quaternion.Euler(0, _currentRotation, 0);
    }

    // Function to select the previous vehicle
    private void SelectPreviousVehicle()
    {
        // Cycle through the vehicles
        _selectedVehicleIndex = (_selectedVehicleIndex - 1 + vehiclePrefabs.Length) % vehiclePrefabs.Length;

        InstantiateSelectedVehicle();
    }

    // Function to select the next vehicle
    private void SelectNextVehicle()
    {
        // Cycle through the vehicles
        _selectedVehicleIndex = (_selectedVehicleIndex + 1) % vehiclePrefabs.Length;

        InstantiateSelectedVehicle();
    }

    // Function to instantiate the selected vehicle
    private void InstantiateSelectedVehicle()
    {
        // Destroy the current vehicle instance if it exists
        if (_currentVehicleInstance != null)
            Destroy(_currentVehicleInstance);

        // Reset material index for the new vehicle
        _currentMaterialIndex = 0;

        // Get the selected vehicle info
        var selectedVehicleInfo = vehiclePrefabs[_selectedVehicleIndex];

        // Instantiate the selected vehicle prefab and store the instance
        _currentVehicleInstance =
            Instantiate(selectedVehicleInfo.DisplayPrefab, transform.position, transform.rotation);

        // Get all renderers from the instantiated vehicle
        _renderers = _currentVehicleInstance.GetComponentsInChildren<Renderer>();

        // Set the current vehicle's materials
        _currentVehicleMaterials = selectedVehicleInfo.VehicleMaterials;

        // Apply the current material to the new vehicle
        if (_currentVehicleMaterials != null && _currentVehicleMaterials.Length > 0)
        {
            ApplyMaterialToVehicle(_currentVehicleMaterials[_currentMaterialIndex]);
        }
        else
        {
            Debug.LogWarning("No materials assigned for the selected vehicle.");
        }
    }

    // Function to load the selected vehicle
    private void LoadSelectedVehicle()
    {
        var selectedVehicleInfo = vehiclePrefabs[_selectedVehicleIndex];
        // Set the player car prefab in the game manager
        GameManager.Instance.SetPlayerCarPrefab(selectedVehicleInfo.GameVehiclePrefab,
            selectedVehicleInfo.VehicleMaterials, _currentMaterialIndex);

        // Load the selected vehicle scene
        SceneManager.LoadScene("DimitriScene");
    }

    // Function to change the vehicle's material
    private void ChangeVehicleMaterial()
    {
        if (_currentVehicleMaterials == null || _currentVehicleMaterials.Length == 0)
        {
            Debug.LogWarning("No materials available to change.");
            return;
        }

        // Cycle through the materials
        _currentMaterialIndex = (_currentMaterialIndex + 1) % _currentVehicleMaterials.Length;

        // Apply the new material
        ApplyMaterialToVehicle(_currentVehicleMaterials[_currentMaterialIndex]);
    }

    // Function to apply a material to all renderers of the vehicle
    private void ApplyMaterialToVehicle(Material newMaterial)
    {
        if (newMaterial == null)
        {
            Debug.LogWarning("Material is null.");
            return;
        }

        foreach (var renderer in _renderers)
        {
            renderer.material = newMaterial;
        }
    }

    [Serializable]
    private class VehicleSelectionInfo
    {
        [SerializeField] private GameObject displayPrefab;
        [SerializeField] private GameObject gameVehiclePrefab;
        [SerializeField] private Material[] vehicleMaterials;

        public GameObject DisplayPrefab => displayPrefab;

        public GameObject GameVehiclePrefab => gameVehiclePrefab;

        public Material[] VehicleMaterials => vehicleMaterials;
    }
}
