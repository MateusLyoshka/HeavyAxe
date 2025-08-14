using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LevelBar : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;

    [SerializeField] private int firstLevelExperience;
    private int currentLevel, totalExperience;
    private float levelMultiplier, nextLevelExperience, experienceRemaining;

    [Header("Interface")]
    [SerializeField] StatusBarText levelText;

    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        currentLevel = 0;
        experienceRemaining = 0;
        totalExperience = 0;
        nextLevelExperience = firstLevelExperience;
        UpdateLevel();
        SetLevelText();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AddExperience(5);
            // Debug.Log("alo");
            // Debug.Log(spaceValue);
        }

    }

    public void AddExperience(int amount)
    {
        totalExperience += amount;
        slider.value = totalExperience;
        CheckLevelUp();
        SetLevelText();
        // slider.value = (int)experienceCurve.Evaluate(totalExperience);
    }

    void CheckLevelUp()
    {
        if (totalExperience >= nextLevelExperience)
        {
            currentLevel++;
            UpdateLevel();
        }
    }

    void UpdateLevel()
    {
        if (currentLevel != 0)
        {
            experienceRemaining = totalExperience - nextLevelExperience;
        }
        slider.value = experienceRemaining;
        totalExperience = (int)experienceRemaining;
        levelMultiplier = experienceCurve.Evaluate(currentLevel + 1);
        nextLevelExperience = levelMultiplier * firstLevelExperience;
        slider.maxValue = nextLevelExperience;
    }

    void SetLevelText()
    {
        string levelStatusString = totalExperience + "/" + (int)nextLevelExperience;
        levelText.SetStatusText(levelStatusString);
    }

}
