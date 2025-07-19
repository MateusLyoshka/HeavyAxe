using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class FakeHeight : MonoBehaviour
{
    [SerializeField] public UnityEvent OnDebrisGroundHit;

    public Transform objectTrsf;
    public Transform bodyTrsf;
    public Transform shadowTrsf;

    public float gravity = -10;
    private Vector2 groundVelocity;
    private float verticalVelocity;

    private bool onGround;

    public void Initialize(Vector2 groundVelocity, float verticalVelocity)
    {
        this.groundVelocity = groundVelocity;
        this.verticalVelocity = verticalVelocity;
    }

    void Update()
    {
        Flying();
    }

    void Flying()
    {
        if (!onGround)
        {
            verticalVelocity += gravity * Time.deltaTime;
            bodyTrsf.position += new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
            CheckGroundHit();
            objectTrsf.position += (Vector3)groundVelocity * Time.deltaTime;
        }
    }

    void CheckGroundHit()
    {
        if (bodyTrsf.position.y < shadowTrsf.position.y)
        {
            bodyTrsf.position = shadowTrsf.position;
            onGround = true;
            GroundHit();
        }
    }

    void GroundHit()
    {
        OnDebrisGroundHit.Invoke();
    }

    public void Sticky()
    {
        groundVelocity = Vector2.zero;
        Destroy(gameObject, 2);
    }
}
