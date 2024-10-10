using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRampHandler : MonoBehaviour
{
    //serialized fields
    [SerializeField][Range(-3,20)]private  float descentSpeed = 3f;        // Descent speed of the car after jumping
    [SerializeField][Range(1,20)]private  float descentDuration = 3f;        // Duration of the descent
    [SerializeField][Range(1,20)]private float airTime = 3f; // Time the car spends in the air after hitting the ramp
    public event Action OnRampEnter;

    public float rampForce = 15f;
    public bool isRamping = false;
    private Rigidbody rb;
    
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //carAnimator = GetComponent<Animator>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ramp"))
        {
            Debug.Log("Car hit the truck ramp!");
            isRamping = true;
            //apply force to simulate ramp
            Vector3 rampDirection = transform.up * rampForce + transform.forward * 1.5f;
            //add force to the car
            rb.AddForce(rampDirection, ForceMode.VelocityChange);
            //invoke event
            OnRampEnter?.Invoke();
            // Directly start the descent sequence after hitting the ramp
            StartCoroutine(TimeAfterJump());
        }
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
        float elapsedTime = 0f;
        float startY = transform.position.y;
        float minY = 0f;
        // Smoothly decrease the Y position over the duration
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(startY, startY - speed, elapsedTime / duration);
            //clamp the y position to a minimum value
            newY = Mathf.Max(newY, minY);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        // Ensure the final Y value is set correctly
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        isRamping = false;  // Reset ramping state
    }
}

    

