using System;
using Unity.VisualScripting;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public event Action OnAxeRotationStoped;

    private Rigidbody2D rb;

    public Transform leftDebrisPoint;
    public Transform rightDebrisPoint;
    public DesbrisDispenser debris;
    public GameObject player;
    private Transform playerTransform;
    private Knight playerScript;

    [SerializeField] private Animator _animator;

    [SerializeField] private float axeWeight = 0.5f;
    [SerializeField] private float maxDistance = 4.5f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float axePullSpeed = 5f;
    [SerializeField] private float rotatingDuration = 0.8f;
    public bool playerCanAttack;

    // Full time variables
    private Vector2 axeToPlayerDirection;
    private float currentDistance;

    // OnRotating || full time variables
    private Vector3 lastPlayerPosition;
    private float playerToAxeAngle, attackAngle;
    private float lastAxeToPlayerAngle;
    private float rotatingTimeElapsed;
    private bool isRotating = false;
    private short rotationDirection = 1;
    private Vector2 lastPlayerToAxeDirection;
    private int axeTurns;
    private int currentTurn;

    // Speed and Damage
    private float speedAtMaxRotation;
    private float speedAtMidRotation;
    private bool midRotationStored;
    private Vector2 lastAxePosition;
    private int acumulatedDamge;
    private float currentSpeed;
    private int damagePerSpeedMult = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = player.GetComponent<Transform>();
        playerScript = player.GetComponent<Knight>();

        // Initialize angle between player and axe at start
        playerToAxeAngle = Mathf.Atan2((transform.position - playerTransform.position).y, (transform.position - playerTransform.position).x) * Mathf.Rad2Deg;
        playerScript.AxeAttack += AxeAttack;
        playerScript.AxeFullSpin += FullSpin;

        lastAxePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Continuously calculate direction from axe to player
        axeToPlayerDirection = playerTransform.position - transform.position;
        playerCanAttack = currentDistance > minDistance;
        if (isRotating)
        {
            AxeRotationCalculator();
        }
        else
        {
            AxeIsNotRotating();
        }

    }

    private void FixedUpdate()
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
                    acumulatedDamge += 50;
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

    void AxeRotationStop()
    {
        // Stop the rotation when the angle between current and target angle is small enough
        if (rotatingTimeElapsed >= rotatingDuration / 2)
        {
            float delta = Mathf.DeltaAngle(playerToAxeAngle, attackAngle);
            bool reachedTarget = Mathf.Abs(delta) <= 3.5f;
            if (reachedTarget && currentTurn == axeTurns)
            {
                currentTurn = 0;
                _animator.SetTrigger("rotationTrigger");
                rotatingTimeElapsed = 0f;
                playerToAxeAngle = attackAngle;
                rotationDirection *= -1;
                OnAxeRotationStoped.Invoke();
                isRotating = false;
                Vector2 debriDirection = (leftDebrisPoint.position - rightDebrisPoint.position).normalized;
                if (rotationDirection == 1 && speedAtMaxRotation >= 18f)
                {
                    debris.DispenserDebris(leftDebrisPoint, debriDirection);
                }
                else if (rotationDirection == -1 && speedAtMaxRotation >= 18) debris.DispenserDebris(rightDebrisPoint, -debriDirection);
                resetSpeedAndDamageVar();
            }
            else if (reachedTarget)
            {
                currentTurn++;
            }
        }
    }

    void AxeRotationCalculator()
    {
        // Calculate rotation progression using time and a power curve for acceleration
        rotatingTimeElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(rotatingTimeElapsed / rotatingDuration);
        float powt2 = Mathf.Pow(0.3f + t, 5f);
        // Adjust the rotation angle depending on rotation direction
        if (rotationDirection == 1) playerToAxeAngle -= powt2;
        else playerToAxeAngle += powt2;
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
        AxeRotationStop();
    }

    void AxeAttack(float attackAngle)
    {
        // Recieve the attack event
        _animator.SetTrigger("rotationTrigger");
        this.attackAngle = attackAngle * Mathf.Rad2Deg;
        axeTurns = 0;
        isRotating = true;
    }

    public void FullSpin(int turns)
    {
        _animator.SetTrigger("rotationTrigger");
        attackAngle = playerToAxeAngle;
        axeTurns = turns;
        isRotating = true;
    }

    void PlayerAxePull()
    {
        // Calculate angle from axe to player to align the axe visually
        float axeToPlayerAngle = Mathf.Atan2((playerTransform.position - transform.position).y, (playerTransform.position - transform.position).x) * Mathf.Rad2Deg;
        rb.SetRotation(axeToPlayerAngle - 42f);

        // If the player is too far, move the axe toward them and apply weight
        currentDistance = axeToPlayerDirection.magnitude;
        if (currentDistance >= maxDistance)
        {
            rb.MovePosition(rb.position + axePullSpeed * Time.deltaTime * axeToPlayerDirection);
            playerScript.ApplyAxeWeight(axeWeight);
        }
        else
        {
            playerScript.ApplyAxeWeight(1f);
        }
    }

    public int ApplyDamage()
    {
        return acumulatedDamge;
    }

    void resetSpeedAndDamageVar()
    {
        // Debug.Log($"mid speed {speedAtMidRotation}, peak speed, {speedAtMaxRotation}");
        speedAtMaxRotation = 0;
        speedAtMidRotation = 0;
        midRotationStored = false;
        acumulatedDamge = 0;
    }
}
