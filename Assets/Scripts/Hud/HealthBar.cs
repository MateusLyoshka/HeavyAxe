using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;

    private float currentStartMaxHealth;
    public int maxLevelHealth = 1800;
    public float healthProgressFactor = 0.4f;
    private int maxLevelPossible = 100;
    void Start()
    {
        slider = GetComponent<Slider>();

    }

    public void HealthStart(int health)
    {

    }

    public void SetMaxHealth(int level)
    {

    }

    public void SetHealth(float health)
    {

    }
}
