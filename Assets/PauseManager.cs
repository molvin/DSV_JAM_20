using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private const string k_pauseString = "Pause";
    public bool gamePaused = false;
    public GameObject CheckerImage;
    private GameObject PauseCanvas;
    public static PauseManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        PauseCanvas = transform.GetChild(0).gameObject;
        GameManager.Instance.OnStartedLoad += unPause;
        CheckerImage.SetActive(PlayerPrefs.GetInt("ControlsInvertedYMultiplier") == -1 ? true : false);
    }
    public void Pause(bool Pause)
    {
        gamePaused = Pause;
        Time.timeScale = Pause ? float.Epsilon : 1;
        PauseCanvas.gameObject.SetActive(Pause);
    }
    public void unPause() => Pause(false);
    private void Update()
    {
        if (Input.GetButtonDown(k_pauseString))
        {
            Pause(!gamePaused);
        }
    }
    public void Restart()
    {
        GameManager.Instance?.GameOver();
    }
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
    public void ToggleInvertedControls()
    {
        int YMultiplier = PlayerPrefs.GetInt("ControlsInvertedYMultiplier", 1);
        PlayerPrefs.SetInt("ControlsInvertedYMultiplier", YMultiplier * -1);
        
        YMultiplier = PlayerPrefs.GetInt("ControlsInvertedYMultiplier");
        Debug.Log(YMultiplier);
        CheckerImage.SetActive(YMultiplier == -1 ? true : false);
        Player.Instance.MovementMachine.GetState<FlyingState>().YcontrolMultiplier = YMultiplier;
    }
}
