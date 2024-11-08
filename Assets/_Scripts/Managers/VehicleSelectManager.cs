using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class VehicleSelectManager : MonoBehaviour
{
    #region Serialized Fields

    // Array to hold vehicle prefabs
    [SerializeField] private VehicleSelectionInfo[] vehiclePrefabs;

    [SerializeField] [Min(0)] private float rotationsPerSecond;

    #endregion

    #region Private Fields

    // Reference to the current vehicle instance
    private GameObject _currentVehicleInstance;

    // Variable to store the selected vehicle index
    private int _selectedVehicleIndex;

    // The current rotation of the vehicle around the y-axis
    private float _currentRotation;

    #endregion

    private void Start()
    {
        // Input manager to subscribe to the swipe event
        InputManager.Instance.OnSwipe += OnSwipeSelectVehicle;

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
        Debug.Log("Selecting previous vehicle");

        // Cycle through the vehicles
        _selectedVehicleIndex = (_selectedVehicleIndex - 1) % vehiclePrefabs.Length;

        if (_selectedVehicleIndex < 0)
            _selectedVehicleIndex += vehiclePrefabs.Length;

        InstantiateSelectedVehicle();
    }

    // Function to select the next vehicle
    private void SelectNextVehicle()
    {
        Debug.Log("Selecting next vehicle");

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

        // Instantiate the selected vehicle prefab and store the instance
        _currentVehicleInstance =
            Instantiate(vehiclePrefabs[_selectedVehicleIndex].DisplayPrefab, transform.position, transform.rotation);
    }

    // Function to load the selected vehicle
    private void LoadSelectedVehicle()
    {
        // Load the selected vehicle scene
        SceneManager.LoadScene("DimitriScene " + _selectedVehicleIndex);
    }

    [Serializable]
    private class VehicleSelectionInfo
    {
        [SerializeField] private GameObject displayPrefab;
        [SerializeField] private GameObject gameVehiclePrefab;

        public GameObject DisplayPrefab => displayPrefab;
        public GameObject GameVehiclePrefab => gameVehiclePrefab;
    }
}