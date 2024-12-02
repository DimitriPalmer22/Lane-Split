using System;
using UnityEngine;

public class CarRampHandler : MonoBehaviour
{
    [Header("Ramp Settings")]
    [SerializeField] private float inclineAngle = 30f;           // Angle of the ramp in degrees
    [SerializeField] private float rampLength = 10f;             // Length of the ramp
    [SerializeField] private float initialSpeed = 5f;            // Speed of the car before hitting the ramp
    [SerializeField] private float frictionCoefficient = 0f;     // Friction coefficient (0 for no friction)

    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 5f;            // Speed of the car when moving forward normally

    public event Action OnRampEnter;
    public event Action OnRampExit;

    private bool _isOnRamp;
    private float _rampTimer;
    private float _acceleration;
    private float _initialVelocity;
    private Vector3 _startPosition;
    private Vector3 _rampDirection;
    private float _gravity = 9.81f;
    private float _inclineAngleRadians;
    private float _distanceAlongRamp;
    private float _totalRampTime;
    
    //properties to expose private fields
    public bool IsOnRamp => _isOnRamp;
    public Vector3 RampDirection => _rampDirection;
    public Vector3 StartPosition => _startPosition;
    public float Acceleration => _acceleration;
    public float InitialVelocity => _initialVelocity;
    public float RampTimer => _rampTimer;
    public float InclineAngle => inclineAngle;
    public float InclineAngleRadians => _inclineAngleRadians;
    

    private void Start()
    {
        // Convert incline angle to radians
        _inclineAngleRadians = inclineAngle * Mathf.Deg2Rad;
    }

    // private void Update()
    // {
    //     if (_isOnRamp)
    //     {
    //         HandleRampMovement();
    //     }
    //     else
    //     {
    //         // Regular forward movement
    //         transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        return;

        if (!other.CompareTag("Ramp"))
            return;

        if (_isOnRamp)
            return;

        Debug.Log("Car hit the ramp!");

        StartRampMovement();

        // Invoke event
        OnRampEnter?.Invoke();
    }

    private void StartRampMovement()
    {
        _isOnRamp = true;
        _rampTimer = 0f;

        // Record the start position
        _startPosition = transform.position;

        // Calculate the direction along the ramp
        _rampDirection = Quaternion.AngleAxis(-inclineAngle, transform.right) * transform.forward;

        // Initial velocity along the ramp
        _initialVelocity = initialSpeed;

        // Calculate acceleration along the incline
        _acceleration = _gravity * (Mathf.Sin(_inclineAngleRadians) - frictionCoefficient * Mathf.Cos(_inclineAngleRadians));

        // Calculate total time to traverse the ramp based on kinematic equations
        // s = v0 * t + 0.5 * a * t^2 => Solve for t when s = rampLength
        _totalRampTime = (-_initialVelocity + Mathf.Sqrt(_initialVelocity * _initialVelocity + 2 * _acceleration * rampLength)) / _acceleration;

        Debug.Log($"Total ramp time: {_totalRampTime}");
    }
    public void UpdateRampTimer(float deltaTime)
    {
        _rampTimer += deltaTime;
    }
    

    private void HandleRampMovement()
    {
        _rampTimer += Time.deltaTime;

        // Calculate current distance along the ramp
        _distanceAlongRamp = _initialVelocity * _rampTimer + 0.5f * _acceleration * _rampTimer * _rampTimer;

        // Prevent overshooting the ramp length
        if (_distanceAlongRamp > rampLength)
            _distanceAlongRamp = rampLength;

        // Calculate new position
        var newPosition = _startPosition + _rampDirection.normalized * _distanceAlongRamp;

        // Update the car's position
        transform.position = newPosition;

        // Optionally adjust rotation to match the incline
        var targetRotation = Quaternion.LookRotation(_rampDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        // Check if the car has reached the end of the ramp
        if (_distanceAlongRamp >= rampLength)
        {
            EndRampMovement();
        }
    }

    private bool _isInAir;
    private Vector3 _airStartPosition;
    private Vector3 _airVelocity;
    private float _airTime;

    // In EndRampMovement()
    private void EndRampMovement()
    {
        _isOnRamp = false;

        // Set up for air movement
        _isInAir = true;
        _airTime = 0f;

        // Calculate velocity at the end of the ramp
        float finalVelocity = _initialVelocity + _acceleration * _rampTimer;

        // Velocity components
        float v_xz = finalVelocity * Mathf.Cos(_inclineAngleRadians);
        float v_y = finalVelocity * Mathf.Sin(_inclineAngleRadians);

        // Air velocity vector
        _airVelocity = _rampDirection.normalized * v_xz + Vector3.up * v_y;

        // Record start position
        _airStartPosition = transform.position;

        // Optionally, adjust rotation
        // Keep the car's current rotation

        Debug.Log("Car has left the ramp and is now in the air.");

        // Invoke event
        OnRampExit?.Invoke();
    }

    // In Update()
    private void Update()
    {
        if (_isOnRamp)
            HandleRampMovement();
        else if (_isInAir)
            HandleAirMovement();
    }

    private void HandleAirMovement()
    {
        _airTime += Time.deltaTime;

        // Update position
        var displacement = _airVelocity * _airTime + 0.5f * Vector3.down * _gravity * _airTime * _airTime;
        transform.position = _airStartPosition + displacement;

        // Check for landing (e.g., when y <= ground level)
        if (transform.position.y <= 0f)
        {
            // Adjust position to ground level
            Vector3 position = transform.position;
            position.y = 0f;
            transform.position = position;

            _isInAir = false;

            // Reset rotation
            Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            transform.rotation = targetRotation;

            Debug.Log("Car has landed.");

            // Continue moving forward
        }
    }

}
