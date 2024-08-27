using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameStateManager : MonoBehaviour
{
    public Button restartButton;
    public GameObject startScreen;
    public Button startButton;
    public TextMeshProUGUI scoreText;
    public PulpitManager platformManager;
    public TextMeshProUGUI finText;


    void Start()
    {
        startScreen.SetActive(true);
        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);
        restartButton.gameObject.SetActive(false);
        scoreText.text = "";
        finText.text = "";
        Time.timeScale = 0f;
    }

    public void GameOver()
    {
        if(platformManager.score>=50)
        {
            finText.text = "Congratulations! You beat the Game!!";
        }
        else
        {
            finText.text = "You died :(";
        }
        restartButton.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        restartButton.gameObject.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        platformManager.Init();
        Time.timeScale = 1f;
        startScreen.SetActive(false);
    }

}
