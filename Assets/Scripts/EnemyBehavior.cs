using UnityEngine;
using UnityEngine.Events;

public class EnemyBehavior : MonoBehaviour
{
    public UnityEvent OnDeath;

    public GameObject enemy;
    private float maxHealth;
    private float currentHealth;

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        currentHealth = health;
        Debug.Log($"vida atual: {currentHealth}");

    }

    public void SetHealth(float health)
    {
        currentHealth = health;
    }

    public void TakeDamage(float damage)
    {
        SetHealth(Mathf.Clamp(currentHealth - damage, 0, maxHealth));
        Debug.Log($"vida atual: {currentHealth}");
        if (currentHealth <= 0)
        {
            DeathBehavior();
        }
    }

    public void DeathBehavior()
    {
        OnDeath.Invoke();
    }
}
