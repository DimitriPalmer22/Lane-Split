using System;
using UnityEngine;

public class LaneBoundsHelper : MonoBehaviour
{
    [SerializeField] private float startZ;
    [SerializeField] private float endZ;
    [SerializeField] private float yOffset;
    [SerializeField] private float lineDistance = 100;

    #region Getters

    public float StartZ => startZ;

    public float EndZ => endZ;

    public float TotalLength => EndZ - StartZ;

    public float YOffset => yOffset;

    public float CurrentStartZ => transform.position.z + startZ;

    #endregion

    private void OnDrawGizmos()
    {
        var startPos = transform.position + new Vector3(0, yOffset, startZ);
        var endPos = transform.position + new Vector3(0, yOffset, endZ);

        // Draw the start
        Gizmos.color = Color.green;
        Gizmos.DrawLine(startPos, startPos + Vector3.up * lineDistance);
        Gizmos.DrawLine(startPos, startPos + Vector3.right * lineDistance);

        // Draw the end
        Gizmos.color = Color.red;
        Gizmos.DrawLine(endPos, endPos + Vector3.up * lineDistance);
        Gizmos.DrawLine(endPos, endPos + Vector3.right * lineDistance);
    }
}