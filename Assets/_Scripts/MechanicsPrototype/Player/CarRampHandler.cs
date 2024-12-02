using UnityEngine;

public class CarRampHandler : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 5f;          // Maximum height of the jump
    [SerializeField] private float jumpDuration = 1f;        // Total duration of the jump

    public bool IsJumping { get; private set; }
    public float JumpTimer { get; private set; }
    public float JumpDuration => jumpDuration;
    public float JumpHeight => jumpHeight;
    
    public event System.Action OnRampEnter;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ramp"))
            return;

        if (IsJumping)
            return;

        Debug.Log("Car hit the ramp!");

        StartJump();
    }

    public void StartJump()
    {
        IsJumping = true;
        JumpTimer = 0f;
        
       // OnRampEnter?.Invoke();
    }

    public void UpdateJumpTimer(float deltaTime)
    {
        if (IsJumping)
        {
            JumpTimer += deltaTime;

            if (JumpTimer >= jumpDuration)
            {
                IsJumping = false;
                JumpTimer = jumpDuration; // Clamp to duration
            }
        }
    }

    public float GetVerticalOffset()
    {
        // Use a parabolic equation to calculate vertical offset
        float normalizedTime = JumpTimer / jumpDuration;
        float height = 4 * jumpHeight * normalizedTime * (1 - normalizedTime); // Parabola

        return height;
    }
}