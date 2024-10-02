using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text boostReadyText;

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

        // Update the boost ready text
        UpdateBoostReadyText();
    }

    private void UpdateBoostReadyText()
    {
        // Return if the boost ready text is null
        if (boostReadyText == null)
            return;

        // Set the text to active if the player has enough boost
        boostReadyText.gameObject.SetActive(LevelManager.Instance.Player.BoostPercentage >= 1);
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

        if (!LevelManager.Instance.Player.IsAlive)
            gameOverText.text = $"Game Over\n" +
                                $"Score: {TestLevelManager.Instance.LevelGenerator.DistanceTravelled}\n" +
                                $"Tap to restart";
    }
}