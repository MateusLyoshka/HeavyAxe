using UnityEngine;
using System;
using System.Collections;

public class Axe : MonoBehaviour
{
    public event Action OnAttackStoped;

    // Axe var
    private Rigidbody2D rb;
    [SerializeField] private AxeShadow axeShadow;
    private Animator _animator;

    // Full time 
    private Vector2 axeToPlayerDirection;
    private float currentPlayerDistance;

    // OnRotating || full time 
    private Vector3 lastPlayerPosition;
    private Vector2 lastPlayerToAxeDirection;
    private float playerToAxeAngle, lastAxeToPlayerAngle;
    private float rotatingTimeElapsed, rotationDirection;

    // Attacks 
    private float previousAngle, accumulatedAngle, attackFinalAngle;
    private bool canAttack;
    private bool isAttacking;

    private PlayerControl playerScript;
    [SerializeField] private SkillManager skillManager;
    private Transform playerTransform;

    [SerializeField] private float axeWeight = 0.5f;
    [SerializeField] private float maxDistance = 4.5f;
    [SerializeField] private float minDistance = 2f;
    private readonly float timeBetweenRotations = 0.1f;
    [SerializeField] private AnimationCurve axeSpeedAnimationCuve;
    [SerializeField] private AnimationCurve axePullSpeedAnimationCuve;

    private int rotationInd;
    private RotationSkillData currentSkill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isAttacking = false;
        rotationDirection = 1f;
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
        axeToPlayerDirection = playerTransform.position - transform.position;


        currentPlayerDistance = axeToPlayerDirection.magnitude;

        if (isAttacking)
        {
            AxeRotationCalculator();
        }
        else
        {
            AxeIsNotRotating();
        }
        // UpdateSpeedCalculation();
    }

    public bool AxePlayerCanAttack()
    {
        return canAttack = currentPlayerDistance > minDistance;
    }

    void AxeIsNotRotating()
    {
        lastPlayerPosition = playerTransform.position;
        lastPlayerToAxeDirection = transform.position - playerTransform.position;
        // Debug.Log($"distancia do pivo pro machado: {lastPlayerToAxeDirection}");
        playerToAxeAngle = Mathf.Atan2(lastPlayerToAxeDirection.y, lastPlayerToAxeDirection.x) * Mathf.Rad2Deg;
        PlayerAxePull();
    }

    void AxeRotationCalculator()
    {
        Debug.Log(lastPlayerToAxeDirection);
        float deltaAngle = Mathf.DeltaAngle(previousAngle, playerToAxeAngle);
        accumulatedAngle += Mathf.Abs(deltaAngle);
        previousAngle = playerToAxeAngle;
        AxeRotationSpeedApply();
        // Debug.Log(playerToAxeAngle);
        // Prevent overflow by flipping angle if it exceeds bounds
        if ((playerToAxeAngle <= -180f && rotationDirection == 1) || (playerToAxeAngle >= 180f && rotationDirection == -1)) playerToAxeAngle *= -1;

        // Calculate the next position along the rotation arc using the fixed radius
        float rad = playerToAxeAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * lastPlayerToAxeDirection.magnitude;
        Vector2 newPos = (Vector2)lastPlayerPosition + offset;
        rb.MovePosition(newPos);

        // Use the last playerTransform.position position before attack to adjust the axe angle
        lastAxeToPlayerAngle = Mathf.Atan2((lastPlayerPosition - transform.position).y, (lastPlayerPosition - transform.position).x) * Mathf.Rad2Deg;
        rb.SetRotation(lastAxeToPlayerAngle - 42f);

        // Check if the rotation should stop
        AxeRotationStopVerify();
    }

    void AxeRotationSpeedApply()
    {
        rotatingTimeElapsed += Time.deltaTime;
        float deltaRotation = axeSpeedAnimationCuve.Evaluate(rotatingTimeElapsed) * Time.deltaTime;
        // Adjust the rotation angle depending on rotation direction
        if (rotationDirection == 1) playerToAxeAngle -= deltaRotation;
        else playerToAxeAngle += deltaRotation;
    }

    void AxeRotationStopVerify()
    {
        // Debug.Log(accumulatedAngle);
        if (accumulatedAngle >= attackFinalAngle)
        {
            _animator.SetTrigger("rotationTrigger");
            rotationDirection *= -1;
            axeShadow.ShadowEndSwing();
            AxeStopResetVar();

            if (rotationInd + 1 < currentSkill.rotationSequence.Count)
            {
                StartCoroutine(WaitAndAttack()); // espera 0.2s antes da próxima
                rotationInd++;
            }
            else
            {
                OnAttackStoped.Invoke();
                isAttacking = false;
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
        if (!isAttacking && AxePlayerCanAttack())
        {
            isAttacking = true;
            rotationInd = 0;
            currentSkill = skill;
            AxeAttack();
        }
    }

    void AxeAttack()
    {
        attackFinalAngle = currentSkill.rotationSequence[rotationInd];
        axeShadow.ShadowStartSwing();
        _animator.SetTrigger("rotationTrigger");
        previousAngle = playerToAxeAngle;
    }

    void AxeStopResetVar()
    {
        // Debug.Log($"mid speed {speedAtMidRotation}, peak speed, {speedAtMaxRotation}");
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
            float axePullSpeed = axePullSpeedAnimationCuve.Evaluate(axeToPlayerDirection.magnitude);
            rb.MovePosition(rb.position + axePullSpeed * Time.deltaTime * axeToPlayerDirection);
            playerScript.ApplyAxeWeight(axeWeight);
        }
        else
        {
            playerScript.ApplyAxeWeight(1f);
        }
    }
}
