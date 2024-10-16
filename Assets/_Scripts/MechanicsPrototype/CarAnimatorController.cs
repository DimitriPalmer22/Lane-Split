using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAnimatorController : MonoBehaviour
{
    [Header("Car Animation References")]
    //reference to the car animator
    [SerializeField]
    private Animator carAnimator;

    //reference to the car ramp handler
    [SerializeField] private CarRampHandler carRampHandler;

    [SerializeField] private TestPlayerScript playerScript;

    private void OnEnable()
    {
        //subscribe to the ramp enter event
        carRampHandler.OnRampEnter += OnRampEnter;
    }

    private void OnDisable()
    {
        //unsubscribe from the ramp enter event
        carRampHandler.OnRampEnter -= OnRampEnter;
    }

    private void OnRampEnter()
    {
        //trigger the jump animation
        carAnimator.SetTrigger("Ramp");

        //double the speed of the ramp animation
        if (playerScript.IsBoosting)
            carAnimator.speed = 2;

        //set the speed of the ramp animation to normal
        else
            carAnimator.speed = 1;
    }
}