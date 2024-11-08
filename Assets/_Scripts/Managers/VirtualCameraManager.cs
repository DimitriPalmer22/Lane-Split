using System;
using Cinemachine;
using UnityEngine;

public class VirtualCameraManager : MonoBehaviour
{
    public static VirtualCameraManager Instance { get; private set; }

    private CinemachineVirtualCamera _virtualCamera;

    public CinemachineVirtualCamera VirtualCamera => _virtualCamera;

    private void Awake()
    {
        // Set the instance
        Instance = this;

        // Get the virtual camera
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        // Set the follow and lookAt of the camera to the player instance
        _virtualCamera.Follow = TestPlayerScript.Instance.transform;
        _virtualCamera.LookAt = TestPlayerScript.Instance.transform;
    }
}