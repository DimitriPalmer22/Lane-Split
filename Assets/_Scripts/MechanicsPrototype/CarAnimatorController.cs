using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAnimatorController : MonoBehaviour
{
    [Header("Car Animation References")]
    //reference to the car animator
    [SerializeField] private Animator carAnimator;
    //reference to the car ramp handler
    [SerializeField] private CarRampHandler carRampHandler;
    
    [SerializeField]private TestPlayerScript playerScript;
    
    private  void OnEnable()
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
        if (playerScript.IsBoosting)
        {
            //double the speed of the ramp animation
            carAnimator.speed = 2;
        }
        else
        {
            //set the speed of the ramp animation to normal
            carAnimator.speed = 1;
        }
    }
}
