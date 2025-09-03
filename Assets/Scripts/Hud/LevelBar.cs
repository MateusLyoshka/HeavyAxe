using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class LevelBar : MonoBehaviour
{
    public event Action<int> levelChange;
    [Header("Experience")]
    private AnimationCurve experienceCurve;

    [SerializeField] private int firstLevelExperience;
    private int currentLevel, totalExperience;
    private float levelMultiplier, nextLevelExperience, experienceRemaining;

    [Header("Interface")]
    private Text levelText;

    private Slider slider;

    public void LevelBarInit(int currentLevel, float levelMultiplier, float experienceRemaining, int firstLevelExperience, AnimationCurve experienceCurve)
    {
        slider = GetComponent<Slider>();
        levelText = GetComponentInChildren<Text>();
        this.currentLevel = currentLevel;
        this.levelMultiplier = levelMultiplier;
        this.experienceRemaining = experienceRemaining;
        this.firstLevelExperience = firstLevelExperience;
        nextLevelExperience = firstLevelExperience;
        totalExperience = (int)experienceRemaining;
        this.experienceCurve = experienceCurve;
        UpdateLevel();
        SetLevelText();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AddExperience(5);
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
            levelChange.Invoke(currentLevel);
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
        levelText.text = levelStatusString;
    }

    public float ReturnExperienceRemaining()
    {
        return experienceRemaining;
    }
}
