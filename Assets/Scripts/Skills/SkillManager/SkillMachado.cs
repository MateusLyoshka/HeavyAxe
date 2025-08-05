using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Machado Giratório")]
public class SkillMachadoGiratorio : SkillBase
{
    public float damage;

    public override void Use(GameObject owner)
    {
        Debug.Log($"{skillName} usada! Dano: {damage}");
        // Aqui você pode adicionar animação, instanciar efeitos, etc.
    }
}