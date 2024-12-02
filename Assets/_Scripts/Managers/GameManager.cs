using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject playerCarPrefab;
    private int _selectedMaterialIndex;
    private Material[] _vehicleMaterials;

    public GameObject PlayerCarPrefab => playerCarPrefab;

    private void Awake()
    {
        // Determine if the instance is null
        if (Instance == null)
        {
            Instance = this;

            // Set the object to not be destroyed when loading a new scene
            DontDestroyOnLoad(gameObject);
        }

        // Destroy the object if it is not null
        // else
        //     Destroy(gameObject);
    }

    public void SetPlayerCarPrefab(GameObject prefab, Material[] material, int materialIndex)
    {
        playerCarPrefab = prefab;
        _vehicleMaterials = material;
        _selectedMaterialIndex = materialIndex;
    }
    
    public GameObject GetPlayerCarPrefab()
    {
        return playerCarPrefab;
    }

    public Material GetVehicleMaterials()
    {
        if(_vehicleMaterials != null && _vehicleMaterials.Length > 0)
        {
            return _vehicleMaterials[_selectedMaterialIndex];
        }
        else
        {
            Debug.LogWarning("No materials assigned for the selected vehicle.");
            return null;
        }    
    }
    
    

}