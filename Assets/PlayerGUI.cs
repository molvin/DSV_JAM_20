using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    public static PlayerGUI Instance;
    public GameObject Holder;
    public Image HealthImage;
    public Image[] CrossHair;
    public float SmoothTime;

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
