using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.PlayerControls.Gameplay.Swerve.performed += MoveCar;
        Debug.Log("Start!");
    }

    // Update is called once per frame
    void Update()
    {
    }


    #region Input Functions

    private void MoveCar(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        
        Debug.Log($"Swerve: {value}");
    }

    #endregion
}