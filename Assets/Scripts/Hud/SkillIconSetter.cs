using UnityEngine;
using UnityEngine.UI;

public class SkillIconSetter : MonoBehaviour
{
    private Image imageComponent;

    void Awake()
    {
        imageComponent = GetComponent<Image>();

    }

    void Start()
    {
    }

    public void SetIcon(Sprite icon)
    {
        imageComponent.sprite = icon;
    }
}
