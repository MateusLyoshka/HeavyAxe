using UnityEngine;
using System;
using System.Collections;

public class Axe : MonoBehaviour
{
    public event Action OnAxeRotationStoped;

    // Axe var
    private Rigidbody2D rb;
    private Animator _animator;

    // Full time 
    private Vector2 axeToPivotDirection;
    private float currentPivotDistance;

    // OnRotating || full time 
    private Vector3 lastPivotPosition;
    private Vector2 lastPivotToAxeDirection;
    private float pivotToAxeAngle, lastAxeToPivotAngle;
    private float rotatingTimeElapsed, rotationDirection = 1;
    private bool isRotating = false;

    // Attacks 
    private float previousAngle, accumulatedAngle, attackFinalAngle;
    private bool canAttack;

    // Speed and Damage
    private Vector2 lastAxePosition;

    private PlayerControl playerScript;
    [SerializeField] private SkillManager skillManager;
    private Transform playerTransform;

    [SerializeField] private float axeWeight = 0.5f;
    [SerializeField] private float axePullSpeed = 10f;
    [SerializeField] private float maxDistance = 4.5f;
    private readonly float rotatingDuration = 1f;
    [SerializeField] private float minDistance = 2f;
    private readonly float timeBetweenRotations = 0.1f;

    private int rotationInd;
    private RotationSkillData currentSkill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerControl>();
        playerTransform = player.GetComponent<Transform>();
        _animator = gameObject.GetComponent<Animator>();
        skillManager.UseSkill += AxeAttackInit;
    }

    // Update is called once per frame
    void Update()
    {
        // Continuously calculate direction from axe to playerTransform.position
        axeToPivotDirection = playerTransform.position - transform.position;
        canAttack = currentPivotDistance > minDistance;

        currentPivotDistance = axeToPivotDirection.magnitude;

        if (isRotating)
        {
            AxeRotationCalculator();
        }
        else
        {
            AxeIsNotRotating();
        }
        // UpdateSpeedCalculation();
    }

    void AxeIsNotRotating()
    {
        lastPivotPosition = playerTransform.position;
        lastPivotToAxeDirection = transform.position - playerTransform.position;
        // Debug.Log($"distancia do pivo pro machado: {lastPivotToAxeDirection}");
        pivotToAxeAngle = Mathf.Atan2(lastPivotToAxeDirection.y, lastPivotToAxeDirection.x) * Mathf.Rad2Deg;
        PlayerAxePull();

    }

    void AxeRotationCalculator()
    {
        float deltaAngle = Mathf.DeltaAngle(previousAngle, pivotToAxeAngle);
        accumulatedAngle += Mathf.Abs(deltaAngle);
        previousAngle = pivotToAxeAngle;
        // Adjust the rotation angle depending on rotation direction
        if (rotationDirection == 1) pivotToAxeAngle -= ReturnRotationSpeed();
        else pivotToAxeAngle += ReturnRotationSpeed();
        // Debug.Log(pivotToAxeAngle);
        // Prevent overflow by flipping angle if it exceeds bounds
        if ((pivotToAxeAngle <= -180f && rotationDirection == 1) || (pivotToAxeAngle >= 180f && rotationDirection == -1)) pivotToAxeAngle *= -1;

        // Calculate the next position along the rotation arc using the fixed radius
        float rad = pivotToAxeAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * lastPivotToAxeDirection.magnitude;
        Vector2 newPos = (Vector2)lastPivotPosition + offset;
        rb.MovePosition(newPos);

        // Use the last playerTransform.position position before attack to adjust the axe angle
        lastAxeToPivotAngle = Mathf.Atan2((lastPivotPosition - transform.position).y, (lastPivotPosition - transform.position).x) * Mathf.Rad2Deg;
        rb.SetRotation(lastAxeToPivotAngle - 42f);

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
        // Debug.Log(accumulatedAngle);
        if (accumulatedAngle >= attackFinalAngle)
        {
            _animator.SetTrigger("rotationTrigger");
            rotationDirection *= -1;
            // OnAxeRotationStoped.Invoke();
            AxeStopResetVar();

            if (rotationInd + 1 < currentSkill.rotationSequence.Count)
            {
                StartCoroutine(WaitAndAttack()); // espera 0.2s antes da próxima
                rotationInd++;
            }
        }
    }

    IEnumerator WaitAndAttack()
    {
        yield return new WaitForSeconds(timeBetweenRotations);
        AxeAttack();
    }

    public void AxeAttackInit(RotationSkillData skill)
    {
        rotationInd = 0;
        currentSkill = skill;
        AxeAttack();
    }

    void AxeAttack()
    {
        attackFinalAngle = currentSkill.rotationSequence[rotationInd];
        _animator.SetTrigger("rotationTrigger");
        isRotating = true;
        previousAngle = pivotToAxeAngle;
    }

    void AxeStopResetVar()
    {
        // Debug.Log($"mid speed {speedAtMidRotation}, peak speed, {speedAtMaxRotation}");
        isRotating = false;
        rotatingTimeElapsed = 0f;
        accumulatedAngle = 0;
    }

    void PlayerAxePull()
    {
        // Calculate angle from axe to player to align the axe visually
        Vector2 axeToPlayerDirection = playerTransform.position - transform.position;
        float axeToPlayerAngle = Mathf.Atan2(axeToPlayerDirection.y, axeToPlayerDirection.x) * Mathf.Rad2Deg;

        rb.SetRotation(axeToPlayerAngle - 42f);

        // If the player is too far, move the axe toward them and apply weight
        if (axeToPlayerDirection.magnitude >= maxDistance)
        {
            rb.MovePosition(rb.position + axePullSpeed * Time.deltaTime * axeToPlayerDirection);
            playerScript.ApplyAxeWeight(axeWeight);
        }
        else
        {
            playerScript.ApplyAxeWeight(1f);
        }
    }
}
