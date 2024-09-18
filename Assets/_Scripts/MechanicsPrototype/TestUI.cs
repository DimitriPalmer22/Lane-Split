using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Update the info text
        UpdateInfoText();
    }

    private void UpdateInfoText()
    {
        var speed = TestLevelManager.Instance.MoveSpeed;
        var distance = TestLevelManager.Instance.LevelGenerator.DistanceTravelled;

        infoText.text = $"Distance: {distance:0.00}\nSpeed: {speed}";
    }
}