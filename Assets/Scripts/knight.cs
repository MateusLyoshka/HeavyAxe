using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class knight : MonoBehaviour
{
    public event Action<float> onSwingAxe;

    public float moveSpeed = 2.5f;
    public axe axeScript;

    private Rigidbody2D rb;
    private Vector2 movement;
    private InputAction moveAction;
    private InputAction attack;

    private float mousePlayerAngle;
    private bool canMouseClick = true;
    private float axeWeight = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attack = InputSystem.actions.FindAction("Attack");
        rb = GetComponent<Rigidbody2D>();
        axeScript.AxeRotationStop += AxeRotationStop;
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
        rb.linearVelocity = axeWeight * moveSpeed * movement.normalized;
    }


    void MouseCapture()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;

        Vector2 direction = mouseWorld - transform.position;
        mousePlayerAngle = Mathf.Atan2(direction.y, direction.x);
        if (attack.WasPressedThisFrame() && canMouseClick)
        {
            canMouseClick = false;
            onSwingAxe?.Invoke(mousePlayerAngle);
        }
    }

    private void AxeRotationStop(bool onRotationStop)
    {
        canMouseClick = onRotationStop;
    }

    public void ApplyAxeWeight(float axeWeight)
    {
        this.axeWeight = axeWeight;
    }

    public float HitAngle()
    {
        return mousePlayerAngle;
    }
}
