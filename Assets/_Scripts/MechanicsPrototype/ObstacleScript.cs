using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    private TestLaneScript _testLaneScript;

    public TestLaneScript TestLaneScript => _testLaneScript;

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
        Gizmos.DrawWireSphere(transform.position, TestLevelManager.Instance.NearMissSize);
    }
}
