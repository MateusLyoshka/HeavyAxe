using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "RotationSkillData", menuName = "RotationSkills/Create Asset")]
public class RotationSkillData : ScriptableObject
{
    [Tooltip("Icon used on interface")]
    public Sprite icon;

    [Tooltip("Name that will be refered on the game")]
    public new string name;

    [Tooltip("Time that the skill needs to be used again. (in seconds)")]
    public float coolDown;

    [Tooltip("Max Damage caused.")]
    public float damage;

    [Tooltip("Rotation action sequence.")]
    public List<float> rotationSequence = new();

}
