using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VehicleSelectManager : MonoBehaviour
{
    // Array to hold vehicle prefabs
    public GameObject[] vehiclePrefabs; // Array to hold vehicle prefabs
    // Reference to the current vehicle instance
    private GameObject currentVehicleInstance; 
    
    // Variable to store the selected vehicle index
    private int selectedVehicle = 0;

    private MenuControls controls;

    // Input system controls
    private void Awake()
    {
        controls = new MenuControls();
        controls.Navigation.PreviousVehicle.performed += _ => SelectPreviousVehicle();
        controls.Navigation.NextVehicle.performed += _ => SelectNextVehicle();
        controls.Navigation.SelectVehicle.performed += _ => LoadSelectedVehicle();
    }

    private void OnEnable()
    {
        controls.Navigation.Enable();
    }

    private void OnDisable()
    {
        controls.Navigation.Disable();
    }

    private void Start()
    {
        InstantiateSelectedVehicle();
    }

    // Function to select the previous vehicle
    private void SelectPreviousVehicle()
    {
        selectedVehicle--;
        if (selectedVehicle < 0)
        {
            selectedVehicle = vehiclePrefabs.Length - 1;
        }
        InstantiateSelectedVehicle();
    }

    // Function to select the next vehicle
    private void SelectNextVehicle()
    {
        selectedVehicle++;
        if (selectedVehicle > vehiclePrefabs.Length - 1)
        {
            selectedVehicle = 0;
        }
        InstantiateSelectedVehicle();
    }

    // Function to instantiate the selected vehicle
    private void InstantiateSelectedVehicle()
    {
        // Destroy the current vehicle instance if it exists
        if (currentVehicleInstance != null)
        {
            Destroy(currentVehicleInstance);
        }

        // Instantiate the selected vehicle prefab and store the instance
        currentVehicleInstance = Instantiate(vehiclePrefabs[selectedVehicle], transform.position, transform.rotation);
    }
    // Function to load the selected vehicle
    private void LoadSelectedVehicle()
    {
        // Load the selected vehicle scene
        SceneManager.LoadScene("DimitriScene" + selectedVehicle);
        
    }
}