using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;



public class CameraSwitchHandler : MonoBehaviour
{
    
    public CinemachineVirtualCamera currentCamera;
    public CinemachineVirtualCamera otherCamera;
    
    private void Start()
    {
        // Set the current camera to the highest priority
        currentCamera.Priority = 20;
        
        // Set the other camera to the lowest priority
        otherCamera.Priority = 10;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("SwitchCamera"))
            return;

        SwitchPriority();
    }

    private void SwitchPriority()
    {
        // Switch the camera priority
        (currentCamera.Priority, otherCamera.Priority) = (otherCamera.Priority, currentCamera.Priority);
    }
   
}
