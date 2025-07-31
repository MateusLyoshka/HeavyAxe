using System;
using Unity.VisualScripting;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public event Action OnAxeRotationStoped;

    private Rigidbody2D rb;
    private SpriteRenderer axeSprt;

    public DesbrisDispenser debris;
    public Transform leftDebrisPoint, rightDebrisPoint;
    private GameObject player;
    private Knight playerScript;
    private Transform playerTransform;

    private Animator _animator;

    [SerializeField] private float axeWeight = 0.5f;
    public float maxDistance = 4.5f;
    [SerializeField] private float unspawnDistance = 6.0f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float axePullSpeed = 5f;
    [SerializeField] private float rotatingDuration = 0.8f;

    // Full time 
    private Vector2 axeToPlayerDirection;
    private float currentDistance;

    // OnRotating || full time 
    private Vector3 lastPlayerPosition;
    private Vector2 lastPlayerToAxeDirection;
    private float playerToAxeAngle, lastAxeToPlayerAngle;
    private float rotatingTimeElapsed, rotationDirection = 1;
    private bool isRotating = false;

    // Attacks 
    private int axeTurns;
    private float previousAngle, accumulatedAngle, attackAngle;
    [HideInInspector] public bool playerCanAttack;
    private bool normalAttack;
    public float minAngleAttack;

    // Speed and Damage
    private Vector2 lastAxePosition;
    private float speedAtMaxRotation, speedAtMidRotation, currentSpeed;
    private int acumulatedDamage;
    private bool midRotationStored;
    private int damagePerSpeedMult = 1;

    // Unspawn and spawn control
    [HideInInspector] public bool axeWillDesapear;
    [HideInInspector] public bool axeInitialized;

    void Awake()
    {
        axeInitialized = false;
        axeSprt = gameObject.GetComponent<SpriteRenderer>();
        // axeSprt.enabled = false;
        rb = GetComponent<Rigidbody2D>();
    }

    public void AxeInit(Knight playerRef, Vector2 spawnPosition, float rotationAngle)
    {
        rb.rotation = rotationAngle - 42f;
        rb.position = spawnPosition;
        playerScript = playerRef;
        player = playerRef.gameObject;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = player.GetComponent<Transform>();
        playerScript = player.GetComponent<Knight>();
        _animator = gameObject.GetComponent<Animator>();

        // Initialize angle between player and axe at start
        playerToAxeAngle = Mathf.Atan2((transform.position - playerTransform.position).y, (transform.position - playerTransform.position).x) * Mathf.Rad2Deg;
        playerScript.AxeAttack += AxeAttack;
        playerScript.AxeFullSpin += FullSpin;

        lastAxePosition = transform.position;

        playerCanAttack = true;
        axeWillDesapear = false;
    }

    private void FixedUpdate()
    {
        if (!axeInitialized)
        {
            axeSprt.enabled = true;
            axeInitialized = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!axeWillDesapear && axeInitialized)
        {
            // Continuously calculate direction from axe to player
            axeToPlayerDirection = playerTransform.position - transform.position;
            playerCanAttack = currentDistance > minDistance;

            currentDistance = axeToPlayerDirection.magnitude;

            if (isRotating)
            {
                AxeRotationCalculator();
            }
            else
            {
                AxeIsNotRotating();
            }
            UpdateSpeedCalculation();
        }
    }

    void UpdateSpeedCalculation()
    {
        if (isRotating)
        {
            float distanceToCurrentPos = Vector2.Distance(lastAxePosition, transform.position);
            currentSpeed = distanceToCurrentPos / Time.fixedDeltaTime;
            if (rotatingTimeElapsed / rotatingDuration >= 0.7 && !midRotationStored)
            {
                speedAtMidRotation = currentSpeed;
                midRotationStored = true;
            }
            if (currentSpeed > speedAtMaxRotation)
            {
                speedAtMaxRotation = currentSpeed;
                if (currentSpeed >= 2 * damagePerSpeedMult)
                {
                    acumulatedDamage += 50;
                    damagePerSpeedMult += 1;
                }
            }
            lastAxePosition = transform.position;
        }
    }

    void AxeIsNotRotating()
    {
        // Store the last known player position and direction to the axe before the attack
        lastAxePosition = transform.position;
        lastPlayerPosition = playerTransform.position;
        lastPlayerToAxeDirection = transform.position - playerTransform.position;
        playerToAxeAngle = Mathf.Atan2(lastPlayerToAxeDirection.y, lastPlayerToAxeDirection.x) * Mathf.Rad2Deg;
        PlayerAxePull();
    }

    void AxeRotationCalculator()
    {
        float deltaAngle = Mathf.DeltaAngle(previousAngle, playerToAxeAngle);
        accumulatedAngle += Mathf.Abs(deltaAngle);
        previousAngle = playerToAxeAngle;
        // Adjust the rotation angle depending on rotation direction
        if (rotationDirection == 1) playerToAxeAngle -= ReturnRotationSpeed();
        else playerToAxeAngle += ReturnRotationSpeed();
        // Prevent overflow by flipping angle if it exceeds bounds
        if ((playerToAxeAngle <= -180f && rotationDirection == 1) || (playerToAxeAngle >= 180f && rotationDirection == -1)) playerToAxeAngle *= -1;

        // Calculate the next position along the rotation arc using the fixed radius
        float rad = playerToAxeAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * lastPlayerToAxeDirection.magnitude;
        Vector2 newPos = (Vector2)lastPlayerPosition + offset;
        rb.MovePosition(newPos);

        // Use the last player position before attack to adjust the axe angle
        lastAxeToPlayerAngle = Mathf.Atan2((lastPlayerPosition - transform.position).y, (lastPlayerPosition - transform.position).x) * Mathf.Rad2Deg;
        rb.SetRotation(lastAxeToPlayerAngle - 42f);

        // Check if the rotation should stop
        AxeRotationStopVerify();
    }

    private float ReturnRotationSpeed()
    {
        // Calculate rotation progression using time and a power curve for acceleration
        rotatingTimeElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(rotatingTimeElapsed / rotatingDuration);
        float powt2 = Mathf.Pow(0.3f + t, 5f);

        return powt2;
    }

    void AxeRotationStopVerify()
    {
        // Stop the rotation when the angle between current and target angle is small enough
        float delta = Mathf.DeltaAngle(playerToAxeAngle, attackAngle);
        bool reachedTarget = Mathf.Abs(delta) <= 5.5f;
        if ((reachedTarget && normalAttack) || (accumulatedAngle >= axeTurns * 360f && !normalAttack))
        {
            AxeStop();
        }
    }

    void AxeAttack(float attackAngle)
    {
        // Recieve the attack event
        this.attackAngle = attackAngle * Mathf.Rad2Deg;
        if (AxeCanAttack())
        {
            _animator.SetTrigger("rotationTrigger");
            axeTurns = 0;
            normalAttack = true;
            isRotating = true;
        }
    }

    public void FullSpin(int turns)
    {
        _animator.SetTrigger("rotationTrigger");
        attackAngle = playerToAxeAngle;
        axeTurns = turns;
        isRotating = true;
        previousAngle = playerToAxeAngle;
    }

    void PlayerAxePull()
    {
        // Calculate angle from axe to player to align the axe visually
        float axeToPlayerAngle = Mathf.Atan2((playerTransform.position - transform.position).y, (playerTransform.position - transform.position).x) * Mathf.Rad2Deg;
        rb.SetRotation(axeToPlayerAngle - 42f);

        // If the player is too far, move the axe toward them and apply weight
        if (currentDistance >= maxDistance && !AxeIsFar())
        {
            rb.MovePosition(rb.position + axePullSpeed * Time.deltaTime * axeToPlayerDirection);
            playerScript.ApplyAxeWeight(axeWeight);
        }
        else
        {
            playerScript.ApplyAxeWeight(1f);
        }
    }

    void AxeStop()
    {

        _animator.SetTrigger("rotationTrigger");
        rotationDirection *= -1;
        Vector2 debriDirection = (leftDebrisPoint.position - rightDebrisPoint.position).normalized;
        if (rotationDirection == 1 && speedAtMaxRotation >= 18f) { debris.DispenserDebris(leftDebrisPoint, debriDirection); }
        else if (rotationDirection == -1 && speedAtMaxRotation >= 18) debris.DispenserDebris(rightDebrisPoint, -debriDirection);
        if (AxeIsFar())
        {
            axeWillDesapear = true;
            playerScript.AxeAttack -= AxeAttack;
            playerScript.AxeFullSpin -= FullSpin;
            transform.SetParent(null, true);
        }
        OnAxeRotationStoped.Invoke();
        AxeStopResetVar();
    }

    void AxeStopResetVar()
    {
        // Debug.Log($"mid speed {speedAtMidRotation}, peak speed, {speedAtMaxRotation}");
        normalAttack = false;
        isRotating = false;
        rotatingTimeElapsed = 0f;
        speedAtMaxRotation = 0;
        speedAtMidRotation = 0;
        midRotationStored = false;
        acumulatedDamage = 0;
        accumulatedAngle = 0;
    }

    public bool AxeIsFar()
    {
        return currentDistance >= unspawnDistance;
    }

    public Vector2 AxeRespawnPosition()
    {
        return transform.position;
    }

    public int ApplyDamage()
    {
        return acumulatedDamage;
    }

    public bool AxeCanAttack()
    {
        float delta = Mathf.DeltaAngle(attackAngle, playerToAxeAngle);
        return Mathf.Abs(delta) > minAngleAttack;
    }
}
