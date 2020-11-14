using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    public Image HealthImage;
    public float SmoothTime;

    private void Update()
    {
        Health health = Player.Instance.GetComponent<Health>();
        float fill = health.CurrentHealth / health.MaxHealth;
        float vel = 0.0f;
        HealthImage.fillAmount = Mathf.SmoothDamp(HealthImage.fillAmount, fill, ref vel, SmoothTime);
    }


}
