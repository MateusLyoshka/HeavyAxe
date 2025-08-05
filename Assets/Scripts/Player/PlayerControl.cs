using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public event Action<float> AxeAttack;
    public event Action<int> AxeFullSpin;
    public event Action OnAxeRotationStarted;


    private Rigidbody2D rb;
    private InputAction attackAction;
    private InputAction moveAction;
    private InputAction dashAction;
    private InputAction axeFullSpinAction;
    private Animator _animator;

    // Dash 
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float dashTimeDelay = 1f;
    [SerializeField] protected float dashDuration = 0.8f;
    [SerializeField] protected float dashForce;

    private float isDashing;
    private float dashTimeDelayPassed;
    private float dashDurationPassed;

    private bool canDash;
    private bool isDashingActive = false;

    // Mouse
    private float mousePlayerAngle;
    private bool canMouseClick = true;
    private float mouseClickWalk;
    private Vector3 lastWorldMousePosition;
    private Vector2 playerMouseDirection;
    private Vector2 lastPlayerMouseDirection;
    private bool lastPMCaptured;

    // Axe
    private Axe axeScript;
    private float axeWeight = 1f;
    private AxeShadow axeShadow;

    // Full axe
    public FullAxe fullAxe;
    private FullAxe instFullAxe;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        dashAction = InputSystem.actions.FindAction("Sprint");
        attackAction = InputSystem.actions.FindAction("Attack");
        axeFullSpinAction = InputSystem.actions.FindAction("Jump");
        moveAction = InputSystem.actions.FindAction("MouseWalk");
        rb = GetComponent<Rigidbody2D>();
        PlayerSpawn();
    }

    private void PlayerSpawn()
    {
        instFullAxe = Instantiate(fullAxe, transform.position, quaternion.identity, transform);
        PlayerAxeRespawn();
        // healthBar = hud.GetComponentInChildren<HealthBar>();

        // currentHealth = maxHealth;
    }

    public void PlayerAxeRespawn()
    {
        axeScript = instFullAxe.GetComponentInChildren<Axe>();
        axeShadow = instFullAxe.GetComponentInChildren<AxeShadow>();
        axeScript.OnAxeRotationStoped += AxeRotationStop;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        MouseCapture();
        SkillButtonsCapture();
    }

    void SkillButtonsCapture()
    {
        float QButton = attackAction.ReadValue<float>();
        float WButton = attackAction.ReadValue<float>();
        float EButton = attackAction.ReadValue<float>();
        float RButton = attackAction.ReadValue<float>();
    }

    void Move()
    {
        isDashing = dashAction.ReadValue<float>();
        mouseClickWalk = moveAction.ReadValue<float>();

        Walk();
        Dash();
    }

    void MouseCapture()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;
        if (mouseClickWalk > 0 && !isDashingActive)
        {
            lastWorldMousePosition = mouseWorld;
            playerMouseDirection = mouseWorld - transform.position;
        }
        mousePlayerAngle = Mathf.Atan2(playerMouseDirection.y, playerMouseDirection.x);
    }

    void Walk()
    {
        float distaceToMousePosition = Vector2.Distance(transform.position, lastWorldMousePosition);
        if (distaceToMousePosition > 0.1f)
        {
            if (!isDashingActive) rb.linearVelocity = axeWeight * moveSpeed * playerMouseDirection.normalized;
        }
        else { rb.linearVelocity = Vector2.zero; }
        _animator.SetBool("isWalking", rb.linearVelocity.magnitude > 0f);

    }

    void Dash()
    {
        if (isDashing == 1 && !isDashingActive && canDash)
        {
            isDashingActive = true;
            canDash = false;
            dashDurationPassed = 0f;
            dashTimeDelayPassed = 0f;
        }
        if (isDashingActive)
        {
            rb.linearVelocity = playerMouseDirection.normalized * dashForce;
            dashDurationPassed += Time.deltaTime;

            if (dashDurationPassed >= dashDuration)
            {
                isDashingActive = false;
                lastPMCaptured = false;
                lastWorldMousePosition = transform.position;
                rb.linearVelocity = Vector2.zero;
            }
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
        // currentHealth -= damage;
        // healthBar.SetHealth(Mathf.Clamp(currentHealth, 0, maxHealth));
    }

}
