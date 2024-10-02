using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugManager : MonoBehaviour, IDebugManaged
{
    public static DebugManager Instance { get; private set; }

    private bool _isDebug;

    [SerializeField] private TMP_Text debugText;

    private List<IDebugManaged> _debugItems;
    
    public bool IsDebug => _isDebug;

    private void Awake()
    {
        // // Set the instance to this object
        // if (Instance == null)
        //     Instance = this;
        // else
        //     Destroy(gameObject);
        //
        // // Set the object to not be destroyed when loading a new scene
        // DontDestroyOnLoad(gameObject);

        // Set the instance to this object
        Instance = this;
        
        // Force the debug flag to false
        SetDebug(true);
        SetDebug(false);

        // Initialize the list of debug Items
        _debugItems = new List<IDebugManaged>();
        
        // Add this to the debug manager
        Instance.AddDebugItem(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Use the input manager to add the debug button
        InputManager.Instance.PlayerControls.Gameplay.Debug.started += OnDebugPressed;
    }

    private void OnDebugPressed(InputAction.CallbackContext obj)
    {
        SetDebug(!_isDebug);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        // Return to avoid errors
        if (!_isDebug)
            return;

        if (debugText == null)
            return;

        if (_debugItems == null)
            return;

        // Reset the text of the debugText
        debugText.text = string.Empty;

        // Write the debug text of each item in the list
        foreach (var item in _debugItems)
            debugText.text += $"{item.GetDebugText()}\n";
    }

    public void SetDebug(bool value)
    {
        if (_isDebug == value)
            return;

        _isDebug = value;

        if (debugText != null)
            debugText.gameObject.SetActive(_isDebug);
    }
    
    public void AddDebugItem(IDebugManaged item)
    {
        if (_debugItems.Contains(item))
            return;

        _debugItems.Add(item);
    }
    
    public void RemoveDebugItem(IDebugManaged item)
    {
        if (!_debugItems.Contains(item))
            return;

        _debugItems.Remove(item);
    }

    public string GetDebugText()
    {
        return "DEBUG VIEW: PRESS F1 TO TOGGLE\n";
    }
}