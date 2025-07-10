using Unity.VisualScripting;
using UnityEngine;

public class axe : MonoBehaviour
{
    private Rigidbody2D rb;

    public GameObject player;
    private Transform playerTransform;
    private kinght playerScript;

    public float axeWeight = 0.7f;
    public float maxDistance = 0.5f;
    public float rotationSpeed = 0.2f;
    public float axeSpeed = 4f;

    private Vector2 direction;
    private float angle;
    private float lastAngle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = player.GetComponent<Transform>();
        playerScript = player.GetComponent<kinght>();
        direction = playerTransform.position - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lastAngle = angle;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            direction = playerTransform.position - transform.position;

            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            lastAngle = Mathf.MoveTowardsAngle(lastAngle, angle, rotationSpeed);
            rb.SetRotation(lastAngle - 42f);
            ApplyRotationAndPull();
        }
    }

    void ApplyRotationAndPull()
    {
        float distance = direction.magnitude;
        if (distance >= maxDistance)
        {
            rb.MovePosition(rb.position + axeSpeed * Time.deltaTime * direction);
            playerScript.ApplyAxeWeight(axeWeight);
        }
        else playerScript.ApplyAxeWeight(1f);
    }
}
