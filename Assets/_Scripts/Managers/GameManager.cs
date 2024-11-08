using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject playerCarPrefab;

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
        else
            Destroy(gameObject);
    }

    public void SetPlayerCarPrefab(GameObject prefab)
    {
        playerCarPrefab = prefab;
    }
}