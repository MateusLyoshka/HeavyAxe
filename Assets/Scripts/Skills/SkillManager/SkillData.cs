using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public float cooldown;

    public abstract void Use(GameObject owner);
}
