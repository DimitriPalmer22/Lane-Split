using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text boostReadyText;
    [SerializeField] private Slider boostSlider;
    [SerializeField] private TMP_Text nearMissText;

    private CountdownTimer _nearMissTimer = new CountdownTimer(.5f);

    // Start is called before the first frame update
    private void Start()
    {
        // Disable the game over text
        gameOverText.gameObject.SetActive(false);

        // Disable the near miss text
        nearMissText.gameObject.SetActive(false);

        LevelManager.Instance.Player.OnNearMiss += (_, _) =>
        {
            nearMissText.gameObject.SetActive(true);

            _nearMissTimer.Reset();
            _nearMissTimer.Start();
        };

        _nearMissTimer.OnTimerEnd += () => nearMissText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        // Update the near miss timer
        _nearMissTimer.Update(Time.deltaTime);

        // Update the info text
        UpdateInfoText();

        // Update the game over text
        UpdateGameOverText();

        // Update the boost ready text
        UpdateBoostReadyText();

        // Update the boost slider
        UpdateBoostSlider();
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

    private void UpdateBoostSlider()
    {
        // Return if the boost slider is null
        if (boostSlider == null)
            return;

        // Set the value of the slider to the boost percentage
        boostSlider.value = LevelManager.Instance.Player.BoostPercentage;
    }
}