using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button StartButton;
    public Button ExitButton;
    private void Start()
    {
        StartButton.onClick.AddListener(StartGame);
        ExitButton.onClick.AddListener(ExitGame);
    }
    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
