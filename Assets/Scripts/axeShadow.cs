using UnityEngine;

public class AxeShadow : MonoBehaviour
{
    public Knight playerScript;
    public Axe axeScript;
    public Transform axeTransform;


    [SerializeField] private Animator _animator;

    void Start()
    {
        playerScript.OnSwingAxe += StartSwing;
        axeScript.OnAxeRotationStop += EndSwing;
    }
    void Update()
    {

    }

    void StartSwing(float attackAngle)
    {
        _animator.SetTrigger("rotationTrigger");
    }

    void EndSwing(bool endSwing)
    {
        _animator.SetTrigger("rotationTrigger");
    }

}
