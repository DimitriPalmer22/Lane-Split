using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Import the new Input System namespace

public class DriftControl : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerControls inputActions; // Reference to input actions
    private Vector2 moveInput;
    private bool isDrifting;
    private float speed = 10.0f;

    private void Awake()
    {
        // Initialize input actions
        inputActions = new PlayerControls();
        inputActions.Gameplay.drift.performed += ctx => StartDrift(); // Subscribe to drift action
        inputActions.Gameplay.drift.canceled += ctx => StopDrift();   // Subscribe to stop drift action
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Propel player forward
    private void FixedUpdate()
    {
        // Continuously apply forward force
        rb.AddForce(-transform.forward * speed, ForceMode.Force);

        // If drifting, rotate the game object 25 degrees on its y-axis
        if (isDrifting)
        {
            transform.Rotate(0, 25 * Time.fixedDeltaTime, 0); // Smooth rotation
        }
    }

    private void StartDrift()
    {
        if (CanDrift()) // Check if the player is in a drift zone
        {
            isDrifting = true;
        }
    }

    private void StopDrift()
    {
        isDrifting = false;
    }

    // Detecting the trigger to enable drift
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DriftZone")) // Assuming the drift area has a tag "DriftZone"
        {
            //animator for drift animation
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DriftZone"))
        {
            isDrifting = false; // Stop drifting when leaving the drift zone
        }
    }

    private bool CanDrift()
    {
        // Your logic to determine if drifting is possible
        return true; // Placeholder
    }
}

