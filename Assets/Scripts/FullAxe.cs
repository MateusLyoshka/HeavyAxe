using Unity.Mathematics;
using UnityEngine;

public class FullAxe : MonoBehaviour
{
    public Axe axePrefab;
    public AxeShadow axeShadowPrefab;

    private Axe axe;
    private AxeShadow axeShadow;

    private Knight player;

    public float destroyTime = 1f;

    private bool axeUnspawned;
    private Vector2 axeSpawnPosition;
    public float axeRespawnTime = 1f;
    private float axeRespawnTimePassed;
    private float axeAngle;

    private void FullAxeInit()
    {
        player = GetComponentInParent<Knight>();
        SpriteRenderer axeSprt = axePrefab.gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer axeShadowSprt = axeShadowPrefab.gameObject.GetComponent<SpriteRenderer>();
        axeSprt.enabled = false;
        axeShadowSprt.enabled = false;
        axe = Instantiate(axePrefab, transform.position, Quaternion.identity, transform);
        axeShadow = Instantiate(axeShadowPrefab, transform.position, Quaternion.identity, transform);
        AxeNewAngle();
        axe.AxeInit(player, axeSpawnPosition, axeAngle);
        axeShadow.AxeShadowInit(axe, player);
        axe.OnAxeRotationStoped += AxeStop;
    }

    private void Awake()
    {
        axeSpawnPosition = new Vector2(transform.position.x + 1f, transform.position.y);
        FullAxeInit();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (axeUnspawned)
        {
            axeRespawnTimePassed += Time.deltaTime;

            if (axeRespawnTimePassed >= axeRespawnTime)
            {
                FullAxeInit();
                player.PlayerAxeRespawn();
                axeRespawnTimePassed = 0f;
                axeUnspawned = false;
            }
        }
    }

    void AxeStop()
    {
        if (axe.AxeIsFar())
        {
            AxeNewPosition();
            Destroy(axe.gameObject, destroyTime);
            Destroy(axeShadow.gameObject, destroyTime);
            axeUnspawned = true;
        }
    }

    void AxeNewPosition()
    {
        Vector2 axeRespawnPosition = axe.AxeRespawnPosition();
        Vector2 playerToAxeDirection = (axeRespawnPosition - (Vector2)player.transform.position).normalized;
        axeSpawnPosition = (Vector2)player.transform.position + playerToAxeDirection * (axe.maxDistance - 1f);
    }

    void AxeNewAngle()
    {
        axeAngle = Mathf.Atan2(((Vector2)player.transform.position - axeSpawnPosition).y, ((Vector2)player.transform.position - axeSpawnPosition).x) * Mathf.Rad2Deg;
    }
}
