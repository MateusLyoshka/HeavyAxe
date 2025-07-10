using Unity.VisualScripting;
using UnityEngine;

public class axe : MonoBehaviour
{
    private Rigidbody2D rb;

    public GameObject player;
    private Transform playerTransform;
    private knight playerScript;

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
        playerScript = player.GetComponent<knight>();
        direction = playerTransform.position - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lastAngle = angle;
        playerTransform.GetComponent<knight>().onSwingAxe += AxeAttack;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            // angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // angle += 0.01f;
            lastAngle = Mathf.MoveTowardsAngle(lastAngle, angle, rotationSpeed);
            rb.SetRotation(lastAngle - 42f);
            // ApplyRotationAndPull();
            // FixedAxe();
        }
    }

    void AxeAttack(float attackAngle)
    {
        direction = playerTransform.position - transform.position;
        float distance = direction.magnitude;
        float centerX = playerTransform.position.x;
        float centerY = playerTransform.position.y;

        float newX = centerX + distance * Mathf.Cos(attackAngle);
        float newY = centerY + distance * Mathf.Sin(attackAngle);
        Vector2 newPosition = new Vector2(newX, newY);
        Debug.Log(newPosition);
        rb.MovePosition(newPosition);

    }

    void FixedAxe()
    {
        float distance = direction.magnitude;
        if (distance != maxDistance)
        {
            rb.MovePosition(playerTransform.position + new Vector3(-1f, -1f, 0f));
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
