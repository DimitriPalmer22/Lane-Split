using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRampHandler : MonoBehaviour
{
    public event Action OnRampEnter;

    public float rampForce = 15f;
    private Rigidbody rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ramp"))
        {
            Debug.Log("Car hit the truck ramp!");
            //apply force to simulate ramp
            Vector3 rampDirection = transform.up * rampForce + transform.forward * 1.5f;
            //add force to the car
            rb.AddForce(rampDirection, ForceMode.VelocityChange);
            //invoke event
            OnRampEnter?.Invoke();
        }
    } 
    
}
