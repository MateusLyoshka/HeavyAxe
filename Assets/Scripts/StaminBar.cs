using System;
using UnityEngine;
using UnityEngine.UI;


public class StaminBar : MonoBehaviour
{
    public Slider slider;
    public Knight playerScript;

    public event Action IsStaminFull;

    public float staminRegenTime;
    public float energyMaxValue;
    private float regenTimePassed;
    private bool isStaminFull;
    private bool isStaminInitialized;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
        playerScript = playerScript.GetComponent<Knight>();
        playerScript.FullSpinResetStamin += ResetStamin;
    }

    public void StaminBarInit(float energyMaxValue, float staminRegenTime)
    {
        slider.maxValue = energyMaxValue;
        this.staminRegenTime = staminRegenTime;
        slider.value = 0;
        isStaminInitialized = true;
    }

    void StaminBarUpdate(float energyAmount)
    {
        slider.value = energyAmount;
        if (slider.value == energyMaxValue)
        {
            IsStaminFull.Invoke();
            isStaminFull = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isStaminFull || !isStaminInitialized) return;
        regenTimePassed += Time.deltaTime;
        float energyAmount = Mathf.Clamp01(regenTimePassed / staminRegenTime);
        StaminBarUpdate(energyAmount);
    }

    void ResetStamin()
    {
        regenTimePassed = 0;
        slider.value = 0;
        isStaminFull = false;
    }
}
