using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusHud : MonoBehaviour
{
    // private event Action OnPlayerDeath;

    [Header("Experience")]
    [Tooltip("Curve of multipliers per level")]
    [SerializeField] private AnimationCurve experienceCurveMultiplier;
    [SerializeField] private AnimationCurve healthCurveMultiplier;

    private HealthBar healtBar;
    private LevelBar levelBar;

    public int currentLevel = 0;
    void Start()
    {
        GameObject healthBarObj = GameObject.FindGameObjectWithTag("HealthBar");
        GameObject levelBarObj = GameObject.FindGameObjectWithTag("LevelBar");
        healtBar = healthBarObj.GetComponent<HealthBar>();
        levelBar = levelBarObj.GetComponent<LevelBar>();

        healtBar.HealthInit(healthCurveMultiplier, 10);
        // Debug.Log(levelBar);
        levelBar.LevelBarInit(currentLevel, experienceCurveMultiplier.Evaluate(currentLevel), 0, 23, experienceCurveMultiplier);
        levelBar.levelChange += LevelUpdated;
        // levelBar.LevelBarStart(currentLevel);
    }

    void LevelUpdated(int newLevel)
    {
        healtBar.UpdateHealth(newLevel);
        Debug.Log("level up");
    }
}
