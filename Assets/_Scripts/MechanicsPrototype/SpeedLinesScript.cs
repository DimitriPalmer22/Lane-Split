using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpeedLinesScript : MonoBehaviour
{
    private VisualEffect _speedLines;

    private void Awake()
    {
        // Get the Visual Effect component for the speed lines
        _speedLines = GetComponent<VisualEffect>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Stop the speed lines by default
        _speedLines.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the speed lines
        UpdateSpeedLines();
    }

    private void UpdateSpeedLines()
    {
        // Determine if the player is boosting
        var isBoosting = LevelManager.Instance.Player.IsBoosting;
        
        // If the player is boosting, play the speed lines
        if (isBoosting)
            _speedLines.Play();
        
        // If the player is not boosting, stop the speed lines
        else
            _speedLines.Stop();
    }
}
