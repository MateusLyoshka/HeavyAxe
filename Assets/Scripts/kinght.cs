using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class kinght : MonoBehaviour
{
    public float moveSpeed = 2.5f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private float mouseClick;
    private InputAction moveAction;
    private InputAction attack;
    private float axeWeight = 1f;
    private Vector2 mousePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attack = InputSystem.actions.FindAction("Attack");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f; // z=0 para garantir que fique no plano 2D
        movement = moveAction.ReadValue<Vector2>();
        mouseClick = attack.ReadValue<float>();
        Debug.Log(mouseClick);
        Move();
    }

    void Move()
    {
        rb.linearVelocity = axeWeight * moveSpeed * movement.normalized;
    }

    public void ApplyAxeWeight(float axeWeight)
    {
        this.axeWeight = axeWeight;
    }

    void Hit()
    {

    }
}
