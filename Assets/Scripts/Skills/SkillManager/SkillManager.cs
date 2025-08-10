using System;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public event Action<RotationSkillData> UseSkill;

    [SerializeField] private RotationSkillData[] skills;
    private PlayerControl playerControl;
    private float[] skillsCoolDown;

    void Start()
    {
        GameObject playerControlObj = GameObject.FindGameObjectWithTag("Player");
        playerControl = playerControlObj.GetComponent<PlayerControl>();
        playerControl.SkillPressed += OnSkillButtonReceived;
        skillsCoolDown = new float[skills.Length];
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] != null) skillsCoolDown[i] = skills[i].coolDown;
            // Debug.Log(skillsCoolDown[i]);

        }

    }

    void Update()
    {
        for (int i = 0; i < skillsCoolDown.Length; i++)
        {
            if (skillsCoolDown[i] > 0)
            {
                skillsCoolDown[i] -= Time.deltaTime;
            }
        }
    }

    void OnSkillButtonReceived(int index)
    {
        if (skills[index] == null) return;
        if (skillsCoolDown[index] <= 0)
        {
            UseSkill.Invoke(skills[index]);
            // Debug.Log(skillsCoolDown[index]);
            skillsCoolDown[index] = skills[index].coolDown;
        }
    }
}
