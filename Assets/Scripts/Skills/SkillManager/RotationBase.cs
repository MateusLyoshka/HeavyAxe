using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class RotationBase : MonoBehaviour
{
    public event Action OnAxeRotationStoped;

    // Axe var
    private GameObject axeObj;
    private Rigidbody2D rb;
    private Transform transform;
    private Animator _animator;

    // SkillsVar
    protected abstract Vector3 Pivot { get; }
    protected abstract float AttackFinalAngle { get; }
    protected abstract float RotatingDuration { get; }
    [SerializeField] private float minDistance = 2f;

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
    private float speedAtMaxRotation, speedAtMidRotation, currentSpeed;
    private int acumulatedDamage;
    private bool midRotationStored;
    private int damagePerSpeedMult = 1;


    // Update is called once per frame
    public void ManualUpdate()
    {
        // Continuously calculate direction from axe to Pivot
        axeToPivotDirection = Pivot - transform.position;
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
        UpdateSpeedCalculation();
    }

    void UpdateSpeedCalculation()
    {
        if (isRotating)
        {
            float distanceToCurrentPos = Vector2.Distance(lastAxePosition, transform.position);
            currentSpeed = distanceToCurrentPos / Time.fixedDeltaTime;
            if (rotatingTimeElapsed / RotatingDuration >= 0.7 && !midRotationStored)
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
        // Store the last known Pivot position and direction to the axe before the attack

    }

    void AxeRotationCalculator()
    {
        float deltaAngle = Mathf.DeltaAngle(previousAngle, pivotToAxeAngle);
        accumulatedAngle += Mathf.Abs(deltaAngle);
        previousAngle = pivotToAxeAngle;
        // Adjust the rotation angle depending on rotation direction
        if (rotationDirection == 1) pivotToAxeAngle -= ReturnRotationSpeed();
        else pivotToAxeAngle += ReturnRotationSpeed();
        // Prevent overflow by flipping angle if it exceeds bounds
        if ((pivotToAxeAngle <= -180f && rotationDirection == 1) || (pivotToAxeAngle >= 180f && rotationDirection == -1)) pivotToAxeAngle *= -1;

        // Calculate the next position along the rotation arc using the fixed radius
        float rad = pivotToAxeAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * lastPivotToAxeDirection.magnitude;
        Vector2 newPos = (Vector2)lastPivotPosition + offset;
        rb.MovePosition(newPos);

        // Use the last Pivot position before attack to adjust the axe angle
        lastAxeToPivotAngle = Mathf.Atan2((lastPivotPosition - transform.position).y, (lastPivotPosition - transform.position).x) * Mathf.Rad2Deg;
        rb.SetRotation(lastAxeToPivotAngle - 42f);

        // Check if the rotation should stop
        AxeRotationStopVerify();
    }

    private float ReturnRotationSpeed()
    {
        // Calculate rotation progression using time and a power curve for acceleration
        rotatingTimeElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(rotatingTimeElapsed / RotatingDuration);
        float powt2 = Mathf.Pow(0.3f + t, 5f);

        return powt2;
    }

    void AxeRotationStopVerify()
    {
        if (accumulatedAngle >= attackFinalAngle)
        {
            _animator.SetTrigger("rotationTrigger");
            rotationDirection *= -1;
            OnAxeRotationStoped.Invoke();
            OnStopOverride();
            AxeStopResetVar();
        }
    }

    void AxeAttack(float attackFinalAngle)
    {
        Debug.Log($"AttackFinalAngle:{AttackFinalAngle}");
        Debug.Log(Pivot);
        // Recieve the attack event
        this.attackFinalAngle = attackFinalAngle;
        _animator.SetTrigger("rotationTrigger");
        isRotating = true;
        previousAngle = pivotToAxeAngle;
    }

    void AxeStopResetVar()
    {
        // Debug.Log($"mid speed {speedAtMidRotation}, peak speed, {speedAtMaxRotation}");
        isRotating = false;
        rotatingTimeElapsed = 0f;
        speedAtMaxRotation = 0;
        speedAtMidRotation = 0;
        midRotationStored = false;
        acumulatedDamage = 0;
        accumulatedAngle = 0;
    }

    public virtual void UseSkill(GameObject axe)
    {
        this.axeObj = axe;
        rb = axeObj.GetComponent<Rigidbody2D>();
        transform = axeObj.GetComponent<Transform>();
        _animator = axeObj.GetComponent<Animator>();
        pivotToAxeAngle = Mathf.Atan2((transform.position - Pivot).y, (transform.position - Pivot).x) * Mathf.Rad2Deg;

        lastAxePosition = transform.position;
        lastPivotPosition = Pivot;
        lastPivotToAxeDirection = transform.position - Pivot;
        Debug.Log($"distancia do pivo pro machado: {lastPivotToAxeDirection}");
        pivotToAxeAngle = Mathf.Atan2(lastPivotToAxeDirection.y, lastPivotToAxeDirection.x) * Mathf.Rad2Deg;

        AxeAttack(AttackFinalAngle);

    }

    protected abstract void OnStopOverride();


}
