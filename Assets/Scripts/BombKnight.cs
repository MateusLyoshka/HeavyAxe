using UnityEngine;
using UnityEngine.UIElements;

public class BombKnight : MonoBehaviour
{
    public Rigidbody2D rb;
    public Knight player;
    public float maxDamage;
    public float minSpeed = 2f;
    public float maxSpeed = 3f;
    public float maxExplosionRadius;

    private Vector2 playerDirection;
    private bool playerCollided;
    private float moveSpeed;
    private float damageTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveSpeed = Random.Range(minSpeed, maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerCollided)
        {
            playerDirection = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = moveSpeed * playerDirection;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            damageTime += Time.deltaTime;
            if (damageTime >= 2f)
            {
                float realDistance = Vector2.Distance(player.transform.position, transform.position);
                if (realDistance < 2f)
                {
                    float t = 1f - (realDistance / maxExplosionRadius);
                    player.DamageReceive(maxDamage * t);
                }
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerCollided = true;
        }
    }
}
