using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [Header("Health")]
    private AnimationCurve healthCurveMultiplier;
    private Slider slider;
    private Text healthText;
    private int currentHealth;
    private int currentMaxHealth;
    private int firstLevelHealth;

    public void HealthInit(AnimationCurve curveMultiplier, int currentMaxHealth)
    {
        slider = GetComponent<Slider>();
        healthText = GetComponentInChildren<Text>();
        healthCurveMultiplier = curveMultiplier;
        this.currentMaxHealth = currentMaxHealth;
        firstLevelHealth = currentMaxHealth;
        UpdateHealth(0);
    }

    private void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        currentMaxHealth = health;
        SetHealthText();
    }

    private void SetHealth(int health)
    {
        slider.value = health;
        currentHealth = health;
        SetHealthText();
    }

    private void SetHealthText()
    {
        healthText.text = (currentHealth + "/" + currentMaxHealth).ToString();
    }

    public void UpdateHealth(int currentLevel)
    {
        float healthMultiplier = healthCurveMultiplier.Evaluate(currentLevel + 1);
        float newExtraHealth = healthMultiplier * firstLevelHealth;
        int newHealth = (int)newExtraHealth;
        SetMaxHealth(newHealth);
        SetHealth(newHealth);
    }
}
