using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRampHandler : MonoBehaviour
{
    //serialized fields
    [SerializeField] [Range(-3, 20)] private float descentSpeed = 3f; // Descent speed of the car after jumping
    [SerializeField] [Range(0.01f, 20)] private float descentDuration = 3f; // Duration of the descent
    [SerializeField] [Range(0.01f, 20)] private float airTime = 3f; // Time the car spends in the air after hitting the ramp

    [SerializeField] private AnimationCurve fallCurve;

    public event Action OnRampEnter;

    public float rampForce = 15f;
    private Rigidbody _rb;

    public bool IsRamping { get; private set; }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        //carAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Car hit {other.name}");

        if (!other.CompareTag("Ramp"))
            return;

        // Check if the car is already ramping
        if (IsRamping)
            return;

        Debug.Log("Car hit the truck ramp!");

        IsRamping = true;

        //apply force to simulate ramp
        var rampDirection = transform.up * rampForce + transform.forward * 1.5f;

        //add force to the car
        _rb.AddForce(rampDirection, ForceMode.VelocityChange);

        //invoke event
        OnRampEnter?.Invoke();

        // Directly start the descent sequence after hitting the ramp
        StartCoroutine(TimeAfterJump());
    }

    private IEnumerator TimeAfterJump()
    {
        // wait for x seconds
        yield return new WaitForSeconds(airTime);

        // Start the descent once time has passed
        StartCoroutine(SmoothDescent(descentSpeed, descentDuration));
    }

    //// Coroutine to handle the smooth descent after the jump
    private IEnumerator SmoothDescent(float speed, float duration)
    {
        Debug.Log("Started Descent");

        var elapsedTime = 0f;
        var startY = transform.position.y;
        var minY = 0f;

        // Smoothly decrease the Y position over the duration
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var newY = Mathf.Lerp(startY, startY - speed, elapsedTime / duration);

            //clamp the y position to a minimum value
            newY = Mathf.Max(newY, minY);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        // Ensure the final Y value is set correctly
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        IsRamping = false; // Reset ramping state
    }
}