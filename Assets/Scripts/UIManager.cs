using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject player;
    public GameObject indicatorsPanel;
    public GameObject UIPanel;
    public GameObject crosshireImg;
    public Slider staminaBar;
    public Slider HealthBar;
    public Slider ManaBar;

    private float maxStamina;
    private float currentStamina;
    private bool isOpened;

    private void Start()
    {
        maxStamina = player.GetComponent<PlayerController>().maxStamina;
        currentStamina = player.GetComponent<PlayerController>().currentStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = currentStamina;
    }

    private void Update()
    {
        currentStamina = player.GetComponent<PlayerController>().currentStamina;
        staminaBar.value = currentStamina;
    }
}
