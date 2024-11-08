using System;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }

    private Volume _volume;

    public Volume Volume => _volume;

    private void Awake()
    {
        // Set the instance
        Instance = this;

        // Get the volume component
        _volume = GetComponent<Volume>();
    }
}