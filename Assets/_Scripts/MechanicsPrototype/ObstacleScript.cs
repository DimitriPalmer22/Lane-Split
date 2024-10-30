using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    [SerializeField] private Transform nearMissPosition;

    private TestLaneScript _testLaneScript;

    public TestLaneScript TestLaneScript => _testLaneScript;

    public Transform NearMissPosition => nearMissPosition;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(TestLaneScript laneScript)
    {
        _testLaneScript = laneScript;
    }

    private void OnDrawGizmos()
    {
        // Skip if there is no level manager yet
        if (TestLevelManager.Instance == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(nearMissPosition.position, TestLevelManager.Instance.NearMissSize);
    }
}
