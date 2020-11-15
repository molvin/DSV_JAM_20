using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class PlayerGUI : MonoBehaviour
{
    public static PlayerGUI Instance;
    public GameObject Holder;
    public Image HealthImage;
    public Image[] CrossHair;
    public float SmoothTime;
    [Header("Score")]
    public float scoreTickSpeed;
    public TextMeshProUGUI score;
    private int currentScore, targetScore;
    public TextMeshProUGUI multiplier;
    public Image livesMask;
    public Color[] colors;
    public int colorProgression;

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        Health health = Player.Instance.GetComponent<Health>();
        float fill = health.CurrentHealth / health.MaxHealth;
        float vel = 0.0f;
        HealthImage.fillAmount = Mathf.SmoothDamp(HealthImage.fillAmount, fill, ref vel, SmoothTime);

        PlayerWeapon[] weapons = Player.Instance.GetComponentsInChildren<PlayerWeapon>();
        for(int i = 0; i < 2; i++)
        {
            Transform target = weapons[i].FindAutoAim();
            if(target == null)
            {
                CrossHair[i].rectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
                CrossHair[i].rectTransform.position = screenPos;

            }
        }

        SetScore(PersistentData.Instance.Score);
        SetMultiplier(PersistentData.Instance.Multiplier);
        SetLives(PersistentData.Instance.Lives);
    }
    public void SetLives(int lives)
    {
        livesMask.fillAmount = lives / 3f;
    }
    public void SetScore(int score)
    {
        targetScore = score;
        currentScore = (int)Mathf.Lerp(currentScore, targetScore, Time.deltaTime * scoreTickSpeed);
        var info = new NumberFormatInfo { NumberGroupSeparator = " "};
        string s = currentScore.ToString("n", info);
        this.score.text = s.Substring(0, s.Length - 3);
    }
    public void SetMultiplier(int value)
    {
        multiplier.text = $"x{value}";
        multiplier.color = colors[value == 0 ? 0 : value  / colorProgression];
    }

    public void Disable()
    {
        Holder.SetActive(false);
    }
    public void Enable()
    {
        Holder.SetActive(true);
    }


}
