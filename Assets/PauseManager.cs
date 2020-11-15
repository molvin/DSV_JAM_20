using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private const string k_pauseString = "Pause";
    public bool gamePaused = false;
    public GameObject CheckerImage;
    private GameObject PauseCanvas;
    public static PauseManager Instance;
    public Button[] ButtonArray;
    private int selectedButton;
    bool updatedSelectedButton;
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
        if(gamePaused)
        {

            if (Input.GetAxis("Acceleration") > 0.5f || Input.GetAxis("Acceleration") < -0.5f || Input.GetAxis("Pitch") > 0.5f || Input.GetAxis("Pitch") < -0.5f)
            {
                if (!updatedSelectedButton)
                {
                    updatedSelectedButton = true;
                    if (Input.GetAxis("Acceleration") > 0.5f || Input.GetAxis("Pitch") > 0.5f)
                        selectedButton -= 1;
                    else if (Input.GetAxis("Acceleration") < -0.5f || Input.GetAxis("Pitch") < -0.5f)
                        selectedButton += 1;

                    selectedButton = (int)Mathf.Clamp(selectedButton, 0, ButtonArray.Length - 1);
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(ButtonArray[selectedButton].gameObject);
                }
            }
            else if (Input.GetAxis("Acceleration") < 0.2f && Input.GetAxis("Acceleration") > -0.2f && Input.GetAxis("Pitch") > 0.2f && Input.GetAxis("Pitch") > -0.2f)
                updatedSelectedButton = false;

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
