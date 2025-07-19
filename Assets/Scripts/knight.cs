using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Knight : MonoBehaviour
{
    public event Action<float> OnSwingAxe;

    [SerializeField] private float moveSpeed = 3f;
    public Axe axeScript;

    private Rigidbody2D rb;
    private Vector2 movement;
    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction dashAction;
    public Animator _animator;

    private float isDashing;
    private float mousePlayerAngle;
    private bool canMouseClick = true;
    private float axeWeight = 1f;

    // Health variables
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    public HealthBar healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dashAction = InputSystem.actions.FindAction("Sprint");
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        rb = GetComponent<Rigidbody2D>();
        axeScript.OnAxeRotationStop += AxeRotationStop;

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        MouseCapture();
    }

    void Move()
    {
        movement = moveAction.ReadValue<Vector2>();
        isDashing = dashAction.ReadValue<float>();
        rb.linearVelocity = axeWeight * moveSpeed * movement.normalized;
        _animator.SetBool("isWalking", rb.linearVelocity.magnitude > 0f);
        if (isDashing == 1)
        {
            rb.linearVelocity = 5f * moveSpeed * movement.normalized;
        }
    }


    void MouseCapture()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;

        Vector2 direction = mouseWorld - transform.position;
        mousePlayerAngle = Mathf.Atan2(direction.y, direction.x);
        if (attackAction.WasPressedThisFrame() && canMouseClick && axeScript.playerCanAttack)
        {
            canMouseClick = false;
            OnSwingAxe?.Invoke(mousePlayerAngle);
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
        healthBar.SetHealth(currentHealth);
    }
}
