using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text gameOverText;

    // Start is called before the first frame update
    void Start()
    {
        // Disable the game over text
        gameOverText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Update the info text
        UpdateInfoText();
        
        // Update the game over text
        UpdateGameOverText();
    }

    private void UpdateInfoText()
    {
        var speed = TestLevelManager.Instance.MoveSpeed;
        var distance = TestLevelManager.Instance.LevelGenerator.DistanceTravelled;

        infoText.text = $"Distance: {distance:0.00}\nSpeed: {speed}";
    }

    private void UpdateGameOverText()
    {
        gameOverText.gameObject.SetActive(!LevelManager.Instance.Player.IsAlive);
    }
}