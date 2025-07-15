using System;
using Unity.VisualScripting;
using UnityEngine;

public class axe : MonoBehaviour
{
    public event Action<bool> AxeRotationStop;

    private Rigidbody2D rb;

    public GameObject player;
    private Transform playerTransform;
    private knight playerScript;

    public float axeWeight = 0.7f;
    public float maxDistance = 1.6f;
    public float minDistance = 1.6f;
    public float axePullSpeed = 4f;
    public float axePushSpeed = 10f;
    public float rotatingDuration = 0.7f;

    // Full time variables
    private Vector2 direction;
    private float currentDistance;
    private float naturalAngle;

    // OnRotating || full time variables
    private Vector3 lastPlayerPosition;
    private float currentAngle, attackAngle;
    private float rotatingTimeElapsed;
    private bool isRotating = false;
    private float attackFixedDistance;
    private bool playerCanAttack;
    private short rotationDirection = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = player.GetComponent<Transform>();
        playerScript = player.GetComponent<knight>();

        Vector2 offset = transform.position - playerTransform.position;
        currentAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

        playerScript.onSwingAxe += AxeAttack;
    }

    // Update is called once per frame
    void Update()
    {
        naturalAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        direction = playerTransform.position - transform.position;
        currentDistance = direction.magnitude;
        if (isRotating)
        {
            AxeRotationCalc();
            rb.SetRotation(naturalAngle - 42f);

            float delta = Mathf.DeltaAngle(currentAngle, attackAngle);
            bool reachedTarget = Mathf.Abs(delta) <= 3.5f;
            if (reachedTarget)
            {
                rotatingTimeElapsed = 0f;
                currentAngle = attackAngle;
                rotationDirection *= -1;
                AxeRotationStop?.Invoke(true);
                isRotating = false;
            }
        }
        else
        {
            lastPlayerPosition = playerTransform.position;
            Vector2 offset = transform.position - playerTransform.position;
            currentAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            ApplyRotationAndPull();
        }

    }

    void AxeRotationCalc()
    {
        rotatingTimeElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(rotatingTimeElapsed / rotatingDuration);
        float powt2 = Mathf.Pow(0.2f + t, 6f);
        if (rotationDirection == 1) currentAngle -= powt2;
        else currentAngle += powt2;
        if ((currentAngle <= -180f && rotationDirection == 1) || (currentAngle >= 180f && rotationDirection == -1)) currentAngle *= -1;
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * attackFixedDistance;
        Vector2 newPos = (Vector2)lastPlayerPosition + offset;

        rb.MovePosition(newPos);
    }

    void AxeAttack(float attackAngle)
    {
        if (playerCanAttack)
        {
            float attacktAngleDeg = attackAngle * Mathf.Rad2Deg;
            this.attackAngle = attacktAngleDeg;
            attackFixedDistance = direction.magnitude;
            isRotating = true;
        }
    }

    void ApplyRotationAndPull()
    {
        rb.SetRotation(naturalAngle - 42f);
        if (currentDistance >= maxDistance)
        {
            rb.MovePosition(rb.position + axePullSpeed * Time.deltaTime * direction);
            playerScript.ApplyAxeWeight(axeWeight);
            playerCanAttack = true;
        }
        else if (currentDistance <= minDistance)
        {
            playerCanAttack = false;
            rb.MovePosition(rb.position + axePushSpeed * Time.deltaTime * -direction);
        }
        else playerScript.ApplyAxeWeight(1f); playerCanAttack = true;

    }
}
