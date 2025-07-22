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
        playerScript.OnSwingAxe += StartSwing;
        axeScript.OnAxeRotationStop += EndSwing;
    }

    void Update()
    {
        transform.position = axeTransform.position + axeOffSet;
        transform.rotation = axeTransform.rotation;
    }
    void StartSwing(float attackAngle)
    {
        _animator.SetTrigger("rotationTrigger");
    }

    void EndSwing()
    {
        _animator.SetTrigger("rotationTrigger");
    }

}
