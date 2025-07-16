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

    [SerializeField] private Animator _animator;

    private float axeWeight = 0.7f;
    private float maxDistance = 2f;
    private float minDistance = 1f;
    private float axePullSpeed = 4f;
    private float rotatingDuration = 0.7f;
    public bool playerCanAttack;

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
        // Time.timeScale = 0.2f;
        naturalAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        direction = playerTransform.position - transform.position;
        currentDistance = direction.magnitude;
        if (isRotating)
        {
            AxeRotationProcess();
            rb.SetRotation(naturalAngle - 42f);
            AxeRotationEnd();
        }
        else
        {
            lastPlayerPosition = playerTransform.position;
            Vector2 offset = transform.position - playerTransform.position;
            currentAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            ApplyRotationAndPull();
        }

    }

    void AxeRotationEnd()
    {
        float delta = Mathf.DeltaAngle(currentAngle, attackAngle);
        bool reachedTarget = Mathf.Abs(delta) <= 3.5f;
        if (reachedTarget)
        {
            _animator.SetTrigger("rotationTrigger");

            rotatingTimeElapsed = 0f;
            currentAngle = attackAngle;
            rotationDirection *= -1;
            AxeRotationStop?.Invoke(true);
            isRotating = false;
        }
    }

    void AxeRotationProcess()
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
            _animator.SetTrigger("rotationTrigger");
            float attacktAngleDeg = attackAngle * Mathf.Rad2Deg;
            this.attackAngle = attacktAngleDeg;
            attackFixedDistance = direction.magnitude;
            isRotating = true;
        }
    }

    void ApplyRotationAndPull()
    {
        rb.SetRotation(naturalAngle - 42f);
        playerCanAttack = currentDistance > minDistance;
        if (currentDistance >= maxDistance)
        {
            rb.MovePosition(rb.position + axePullSpeed * Time.deltaTime * direction);
            playerScript.ApplyAxeWeight(axeWeight);
        }
        else
        {
            playerScript.ApplyAxeWeight(1f);
        }
    }
}
