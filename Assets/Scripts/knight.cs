using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Knight : MonoBehaviour
{
    public event Action<float> AxeAttack;
    public event Action<int> AxeFullSpin;
    public event Action OnAxeRotationStarted;
    public event Action FullSpinResetStamin;

    [SerializeField] private float moveSpeed = 3f;
    public Axe axeScript;

    private Rigidbody2D rb;
    private Vector2 movement;
    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction dashAction;
    private InputAction axeFullSpinAction;
    public Animator _animator;

    private float isDashing;
    public float dashTimeDelayPassed;
    public float dashTimeDelay = 1f;
    public float dashDuration = 0.8f;
    private float dashDurationPassed;
    public float dashForce;
    private bool canDash;
    private bool isDashingActive = false;

    private float mousePlayerAngle;
    private bool canMouseClick = true;
    private float axeWeight = 1f;

    // Health var
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    public HealthBar healthBar;

    // Stamin var
    public float staminRegenTime = 4f;
    public float energyMaxValue = 1f;
    private bool isStaminFull;
    public StaminBar staminBar;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dashAction = InputSystem.actions.FindAction("Sprint");
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        axeFullSpinAction = InputSystem.actions.FindAction("Jump");

        rb = GetComponent<Rigidbody2D>();
        axeScript.OnAxeRotationStoped += AxeRotationStop;

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        staminBar.StaminBarInit(energyMaxValue, staminRegenTime);
        staminBar.IsStaminFull += IsStaminFull;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        MouseCapture();
    }

    void Move()
    {
        isDashing = dashAction.ReadValue<float>();

        if (isDashing == 1 && !isDashingActive && canDash)
        {
            isDashingActive = true;
            canDash = false;
            dashDurationPassed = 0f;
            dashTimeDelayPassed = 0f;
        }
        if (isDashingActive)
        {
            rb.linearVelocity = movement.normalized * dashForce;
            dashDurationPassed += Time.deltaTime;

            if (dashDurationPassed >= dashDuration)
            {
                isDashingActive = false;
            }
        }
        else
        {
            movement = moveAction.ReadValue<Vector2>();
            rb.linearVelocity = axeWeight * moveSpeed * movement.normalized;
            _animator.SetBool("isWalking", rb.linearVelocity.magnitude > 0f);
        }
        if (!canDash)
        {
            dashTimeDelayPassed += Time.deltaTime;
            if (dashTimeDelayPassed >= dashTimeDelay)
            {
                canDash = true;
            }
        }
    }

    void MouseCapture()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;

        Vector2 direction = mouseWorld - transform.position;
        mousePlayerAngle = Mathf.Atan2(direction.y, direction.x);
        if (canMouseClick && axeScript.playerCanAttack)
        {
            if (attackAction.WasPressedThisFrame())
            {
                canMouseClick = false;
                AxeAttack?.Invoke(mousePlayerAngle);
                OnAxeRotationStarted.Invoke();
            }
            else if (axeFullSpinAction.WasPressedThisFrame() && isStaminFull)
            {
                canMouseClick = false;
                AxeFullSpin?.Invoke(2);
                OnAxeRotationStarted.Invoke();
                FullSpinResetStamin.Invoke();
                isStaminFull = false;
            }

        }
    }

    private void AxeRotationStop()
    {
        canMouseClick = true;
    }

    public void ApplyAxeWeight(float axeWeight)
    {
        this.axeWeight = axeWeight;
    }

    public float HitAngle()
    {
        return mousePlayerAngle;
    }

    public void DamageReceive(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(Mathf.Clamp(currentHealth, 0, maxHealth));
    }

    private void IsStaminFull()
    {
        isStaminFull = true;
    }
}
