using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelBar : MonoBehaviour
{
    private Slider slider;
    private LevelText levelText;
    public event Action OnMaxExp;

    private int currentLevel;
    private float currentExp;
    private float levelUpExp;
    public float nextLevelExpMultiplier;

    // void Start()
    // {
    //     slider = GetComponent<Slider>();
    //     levelText = GetComponentInChildren<LevelText>();
    //     currentLevel = 0;
    //     currentExp = 0;
    //     levelUpExp = 20;
    // }

    // public void LevelBarStart(int currentLevel)
    // {
    //     slider.maxValue = 0;
    //     levelUpExp = 20;
    //     currentExp = 0;
    //     slider.maxValue = levelUpExp;
    //     slider.value = currentExp;
    // }

    // public void LevelUp(int level)
    // {
    //     currentLevel = level;
    //     levelText.SetLevel(level);
    //     slider.maxValue = levelUpExp * nextLevelExpMultiplier;
    //     slider.value = 0;
    // }

    // public void AddExp(float exp)
    // {
    //     slider.value = exp;
    //     if (currentExp == levelUpExp)
    //     {
    //         OnMaxExp.Invoke();
    //     }
    // }
}
