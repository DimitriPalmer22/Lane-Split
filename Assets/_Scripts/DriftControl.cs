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
    public float speed = 10.0f;
    private float driftRotationSpeed = 100.0f; // Increase rotation speed for a tighter drift
    private float driftForceMultiplier = 1.5f; // Extra force to simulate tighter control
    
    private float minRotation = 0f;
    private float maxRotation = 360f;

    //public float straightenSpeed = 1f;
    //public Quaternion targetRotation;
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
        rb.AddForce(transform.forward * speed, ForceMode.Force);

        if (isDrifting)
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.y = Mathf.Clamp(currentRotation.y,minRotation,maxRotation);
            transform.eulerAngles = currentRotation; 
            // Rotate the game object while drifting
            transform.Rotate(0, driftRotationSpeed * Time.fixedDeltaTime, 0); 

            // Optionally apply additional force for drifting effects
            Vector3 driftForce = transform.right * driftForceMultiplier;
            rb.AddForce(driftForce, ForceMode.Acceleration);
        }
        

        
        // if()
        {
            // // Gradually decrease the forward speed after drifting
            // speed = Mathf.Lerp(speed, 0, Time.fixedDeltaTime * 2);

            // Rotate the car back to its original direction gradually
            // transform.rotation = Quaternion.lerp();

            // // Clamp the rotation to prevent excessive spinning
            // Vector3 clampedEulerAngles = transform.eulerAngles;
            // clampedEulerAngles.y = Mathf.Clamp(clampedEulerAngles.y, -120f, 120f); // Example limits
            // transform.eulerAngles = clampedEulerAngles;
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
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("DriftZone")) // Assuming the drift area has a tag "DriftZone"
    //     {
    //         //animator for drift animation
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("DriftZone"))
    //     {
    //         isDrifting = false; // Stop drifting when leaving the drift zone
    //     }
    // }

    private bool CanDrift()
    {
        return true; // Placeholder
    }
}

