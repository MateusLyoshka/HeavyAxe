using UnityEngine;
using UnityEngine.UI;

public class SkillIconSetter : MonoBehaviour
{
    private Image imageComponent;

    void Start()
    {
        imageComponent = GetComponent<Image>();
    }

    public void SetIcon(Sprite icon)
    {
        imageComponent.sprite = icon;
    }
}
