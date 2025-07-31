using UnityEngine;

public class AxeShadow : MonoBehaviour
{
    public SpriteRenderer axeShadowSprite;

    private Knight player;
    private Axe axe;
    private Transform axeTransform;

    [SerializeField] private Animator _animator;
    [SerializeField] private Vector3 axeOffSet = new(0, -0.2f, 0);

    public void AxeShadowInit(Axe axeRef, Knight playerRef)
    {
        axe = axeRef;
        axeTransform = axe.transform;
        player = playerRef;
    }

    void Start()
    {
        player.OnAxeRotationStarted += StartSwing;
        axe.OnAxeRotationStoped += EndSwing;
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

        if (axe.axeWillDesapear)
        {
            transform.SetParent(null, true);
        }
    }

}
