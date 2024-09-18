using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}