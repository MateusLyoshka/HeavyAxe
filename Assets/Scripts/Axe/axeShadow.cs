using UnityEngine;

public class AxeShadow : MonoBehaviour
{
    public SpriteRenderer axeShadowSprite;

    private PlayerControl player;
    private Axe axe;
    private Transform axeTransform;

    [SerializeField] private Animator _animator;
    [SerializeField] private Vector3 axeOffSet = new(0, -0.2f, 0);

    private bool shadowInitialized;

    public void AxeShadowInit(Axe axeRef, PlayerControl playerRef)
    {
        axe = axeRef;
        axeTransform = axe.transform;
        player = playerRef;
        SpriteRenderer axeShadowSprt = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        player.OnAxeRotationStarted += StartSwing;
        axe.OnAxeRotationStoped += EndSwing;
    }

    void FixedUpdate()
    {
        axeShadowSprite.enabled = true;
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
            player.OnAxeRotationStarted -= StartSwing;
            transform.SetParent(null, true);
        }
    }

}
