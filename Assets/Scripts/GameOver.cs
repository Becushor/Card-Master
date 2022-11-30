using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text killsText = null;
    public Text scoreText = null;

    private void Awake()
    {
        killsText.text = "Demons Killed: " + GameController.instance.playerKills.ToString();
        scoreText.text = "Score: " + GameController.instance.playerScore.ToString();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
