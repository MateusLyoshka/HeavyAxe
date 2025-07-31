using System;
using UnityEngine;
using UnityEngine.UI;


public class StaminBar : MonoBehaviour
{
    public Slider slider;
    public GameObject player;
    private Knight playerScript;

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
    }

    public void StaminBarInit(float energyMaxValue, float staminRegenTime, Knight playerRef)
    {
        playerScript = playerRef;
        playerScript.FullSpinResetStamin += ResetStamin;
        slider.maxValue = energyMaxValue;
        this.staminRegenTime = staminRegenTime;
        slider.value = 0;
        isStaminInitialized = true;
        isStaminFull = false;
    }

    public void SetStamin(float energyValue)
    {
        slider.value = energyValue;
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
