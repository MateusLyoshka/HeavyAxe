using UnityEngine;

public class AxeShadow : MonoBehaviour
{
    public SpriteRenderer axeShadowSprite;

    public Knight playerScript;
    public Axe axeScript;
    public Transform axeTransform;


    [SerializeField] private Animator _animator;

    [SerializeField] private Vector3 axeOffSet = new(0, -0.2f, 0);


    void Start()
    {
        playerScript.OnAxeRotationStarted += StartSwing;
        axeScript.OnAxeRotationStoped += EndSwing;
    }

    void Update()
    {
        transform.SetPositionAndRotation(axeTransform.position + axeOffSet, axeTransform.rotation);
    }
    void StartSwing()
    {
        _animator.SetTrigger("rotationTrigger");
    }

    void EndSwing()
    {
        _animator.SetTrigger("rotationTrigger");
    }

}
