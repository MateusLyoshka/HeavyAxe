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

    private SpriteRenderer axeSprt;
    private SpriteRenderer axeShadowSprt;

    private bool axeUnspawned;
    public float axeRespawnTime = 1f;
    private float axeRespawnTimePassed;

    private void Awake()
    {
        axe = Instantiate(axePrefab, transform.position, Quaternion.identity, transform);
        axeShadow = Instantiate(axeShadowPrefab, transform.position, Quaternion.identity, transform);
        player = GetComponentInParent<Knight>();
        axe.axeInit(player);
        axeShadow.AxeShadowInit(axe, player);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        axeSprt = axe.GetComponent<SpriteRenderer>();
        axeShadowSprt = axeShadow.GetComponent<SpriteRenderer>();

        axe.OnAxeRotationStoped += AxeStop;
    }

    // Update is called once per frame
    void Update()
    {
        if (axeUnspawned)
        {
            axeRespawnTimePassed += Time.deltaTime;

            if (axeRespawnTimePassed >= axeRespawnTime)
            {
                axe = Instantiate(axePrefab, transform.position, Quaternion.identity, transform);
                axeShadow = Instantiate(axeShadowPrefab, transform.position, Quaternion.identity, transform);
                axeShadow.AxeShadowInit(axe, player);
                axe.axeInit(player);
                axeUnspawned = false;
            }
        }
    }

    void AxeStop()
    {
        if (axe.AxeIsFar())
        {
            Destroy(axe.gameObject, destroyTime);
            Destroy(axeShadow.gameObject, destroyTime);
            axeUnspawned = true;
        }
    }
}
