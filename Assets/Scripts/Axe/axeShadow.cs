using UnityEngine;

public class AxeShadow : MonoBehaviour
{
    public SpriteRenderer axeShadowSprite;

    public Transform axeTransform;

    [SerializeField] private Animator _animator;
    [SerializeField] private Vector3 axeOffSet = new(0, -0.2f, 0);

    void Update()
    {
        transform.SetPositionAndRotation(axeTransform.position + axeOffSet, axeTransform.rotation);
    }
    public void ShadowStartSwing()
    {
        _animator.SetTrigger("rotationTrigger");
    }

    public void ShadowEndSwing()
    {
        _animator.SetTrigger("rotationTrigger");
    }

}
