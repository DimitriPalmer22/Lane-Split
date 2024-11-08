using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VehicleSelectManager : MonoBehaviour
{
    // Array to hold vehicle prefabs
    public GameObject[] vehiclePrefabs;

    // Reference to the current vehicle instance
    private GameObject _currentVehicleInstance;

    // Variable to store the selected vehicle index
    private int _selectedVehicleIndex;

    private MenuControls _controls;

    // Input system controls
    private void Awake()
    {
        _controls = new MenuControls();

        _controls.Navigation.PreviousVehicle.performed += _ => SelectPreviousVehicle();
        _controls.Navigation.NextVehicle.performed += _ => SelectNextVehicle();
        _controls.Navigation.SelectVehicle.performed += _ => LoadSelectedVehicle();
    }

    private void OnEnable()
    {
        _controls.Navigation.Enable();
    }

    private void OnDisable()
    {
        _controls.Navigation.Disable();
    }

    private void Start()
    {
        InstantiateSelectedVehicle();
    }

    // Function to select the previous vehicle
    private void SelectPreviousVehicle()
    {
        _selectedVehicleIndex--;
        if (_selectedVehicleIndex < 0)
        {
            _selectedVehicleIndex = vehiclePrefabs.Length - 1;
        }

        InstantiateSelectedVehicle();
    }

    // Function to select the next vehicle
    private void SelectNextVehicle()
    {
        _selectedVehicleIndex++;
        if (_selectedVehicleIndex > vehiclePrefabs.Length - 1)
        {
            _selectedVehicleIndex = 0;
        }

        InstantiateSelectedVehicle();
    }

    // Function to instantiate the selected vehicle
    private void InstantiateSelectedVehicle()
    {
        // Destroy the current vehicle instance if it exists
        if (_currentVehicleInstance != null)
        {
            Destroy(_currentVehicleInstance);
        }

        // Instantiate the selected vehicle prefab and store the instance
        _currentVehicleInstance = Instantiate(vehiclePrefabs[_selectedVehicleIndex], transform.position, transform.rotation);
    }

    // Function to load the selected vehicle
    private void LoadSelectedVehicle()
    {
        // Load the selected vehicle scene
        SceneManager.LoadScene("DimitriScene " + _selectedVehicleIndex);
    }
}