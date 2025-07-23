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
    private bool playerCollided;
    private float moveSpeed;
    private float damageTime = 0f;
    public float maxHealth;
    private int receivedDamage;
    private bool isDead;


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
        if (!isDead)
        {
            playerDirection = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = moveSpeed * playerDirection;
        }
        else
        {

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
            // Debug.Log("a");
        }
    }

    public void ExplosionDeath()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;

        Debug.Log("oi explosion");

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

    }

    public void TakeDamageBehavior()
    {
        receivedDamage = axe.ApplyDamage();
        enemyBehavior.TakeDamage(receivedDamage);
        Debug.Log($"dano recebido {receivedDamage}");
        Vector2 knockbackDir = (transform.position - player.transform.position).normalized;
        rb.linearVelocity = knockbackDir * 50f;
    }
}
