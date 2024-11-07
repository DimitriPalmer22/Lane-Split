using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoostTiming : MonoBehaviour
{
    public float successWindow = 1.0f; // Time window for each successful input
    private float timeToReact;
    public bool isQTEActive = false;
    private float TimedBoostMultiplier = 1.5f;

    // Reference to the TestPlayerScript
    public TestPlayerScript playerScript;

    //reference to the PlayerControls input action
    public PlayerControls playerControls;

    // Track number of successful inputs
    [SerializeField]private int successfulInputs = 0;
    [SerializeField]private const int requiredInputs = 2;

    private void Awake()
    {
        playerScript = GetComponent<TestPlayerScript>();
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        // Subscribe to the input action
       // playerControls.Gameplay.ActivateQTE.performed += OnInput;
    }

    private void OnDisable()
    {
        // Unsubscribe from the input action
       // playerControls.Gameplay.ActivateQTE.performed -= OnInput;
    }

    private void Update()
    {
        if (isQTEActive && Time.time > timeToReact)
        {
            FailQTE();
        }
    }

    public void StartQTE()
    {
        if (isQTEActive) return; // Prevent starting a new QTE if one is already active
        isQTEActive = true;
        successfulInputs = 0; // Reset successful input counter
        timeToReact = Time.time + successWindow; // Set reaction time for the first input
        Debug.Log("QTE Started!"); // Optional: Log QTE start
    }

    public void OnInput(InputAction.CallbackContext context)
    {
        if (context.performed && isQTEActive)
        {
            successfulInputs++;
            Debug.Log("Input Successful! Count: " + successfulInputs);

            if (successfulInputs >= requiredInputs)
            {
                SuccessQTE();
            }
            else
            {
                // Update the time for the next input window
                timeToReact = Time.time + successWindow;
            }
        }
    }

    private void SuccessQTE()
    {
        isQTEActive = false; // End the QTE
       // playerScript.ChangeBoostMultiplier(TimedBoostMultiplier); // Change boost multiplier
        //playerScript.Boost(); // Call existing Boost() method to activate the boost
        Debug.Log("QTE Success! Boost Activated.");
    }

    private void FailQTE()
    {
        isQTEActive = false; // End the QTE
        Debug.Log("QTE Failed! Boost not activated.");
    }
}
