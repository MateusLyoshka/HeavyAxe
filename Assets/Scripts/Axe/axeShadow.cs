using UnityEngine;

public class AxeShadow : MonoBehaviour
{
    public SpriteRenderer axeShadowSprite;

    private PlayerControl player;
    public Axe axe;
    public Transform axeTransform;

    [SerializeField] private Animator _animator;
    [SerializeField] private Vector3 axeOffSet = new(0, -0.2f, 0);

    void Start()
    {
        // player.OnAxeRotationStarted += StartSwing;
        // axe.OnAxeRotationStoped += EndSwing;
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
