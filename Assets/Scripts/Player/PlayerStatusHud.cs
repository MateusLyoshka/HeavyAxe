using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusHud : MonoBehaviour
{
    // private event Action OnPlayerDeath;

    private HealthBar healtBar;
    private LevelBar levelBar;

    public int currentLevel = 0;
    void Start()
    {
        GameObject healthBarObj = GameObject.FindGameObjectWithTag("HealthBar");
        GameObject levelBarObj = GameObject.FindGameObjectWithTag("LevelBar");
        healtBar = healthBarObj.GetComponent<HealthBar>();
        levelBar = levelBarObj.GetComponent<LevelBar>();

        healtBar.HealthStart(currentLevel);
        // levelBar.LevelBarStart(currentLevel);
    }
}
