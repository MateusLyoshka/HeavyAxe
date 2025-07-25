using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class BombKnight : MonoBehaviour
{
    public Rigidbody2D rb;
    private Knight player;
    private Axe axe;
    public EnemyBehavior enemyBehavior;
    public DesbrisDispenser debris;
    public float maxDamage;
    public float minSpeed = 2f;
    public float maxSpeed = 3f;
    public float maxExplosionRadius;

    private Vector2 playerDirection;
    private float moveSpeed;
    private float knockbackTime = 0f;
    private bool onKnockback, isDead, playerCollided;
    public float maxHealth;
    private int receivedDamage;
    private float explosionTime;
    public float knokbackPower = 10f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        GameObject axeObject = GameObject.FindWithTag("Axe");
        player = playerObject.GetComponent<Knight>();
        axe = axeObject.GetComponent<Axe>();
        enemyBehavior.SetMaxHealth(maxHealth);
        moveSpeed = Random.Range(minSpeed, maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead && !onKnockback && !playerCollided)
        {
            playerDirection = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = moveSpeed * playerDirection;
        }
        else if (playerCollided)
        {
            explosionTime += Time.deltaTime;
            if (explosionTime >= 2f)
            {
                ExplosionDeath();
                explosionTime = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerCollided = true;
        }
        if (collision.gameObject.CompareTag("Axe"))
        {
            TakeDamageBehavior();
        }
    }

    public void ExplosionDeath()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;

        float realDistance = Vector2.Distance(player.transform.position, transform.position);
        if (realDistance < maxExplosionRadius)
        {
            float t = 1f - (realDistance / maxExplosionRadius);
            player.DamageReceive(maxDamage * t);
        }
        debris.DispenserDebris(transform, Random.insideUnitCircle);
        Destroy(gameObject);
    }

    public void DeathByAttack()
    {
        isDead = true;
        float randomExplosion = Random.Range(0, 4);
        if (randomExplosion == 0)
        {
            ExplosionDeath();
        }
        else
        {
            Destroy(gameObject, 0.5f);
        }
    }

    public void TakeDamageBehavior()
    {
        receivedDamage = axe.ApplyDamage();
        enemyBehavior.TakeDamage(receivedDamage);
        if (receivedDamage > 0)
        {
            StartCoroutine(ApplyKnockback());
        }
    }

    private IEnumerator ApplyKnockback()
    {
        onKnockback = true;
        Vector2 knockbackDir = (transform.position - player.transform.position).normalized;
        rb.AddForce(knockbackDir * knokbackPower, ForceMode2D.Impulse);
        while (knockbackTime <= 0.5f)
        {
            knockbackTime += Time.deltaTime;
            yield return null;
        }
        onKnockback = false;
        knockbackTime = 0;
    }
}
