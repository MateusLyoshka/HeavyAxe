using System;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public event Action<RotationSkillData> UseSkill;

    [SerializeField] private RotationSkillData[] skillsData;
    [SerializeField] private SingularSkill[] hudSkillScript;
    private PlayerControl playerControl;
    private float[] skillsCoolDown;

    void Start()
    {
        GameObject playerControlObj = GameObject.FindGameObjectWithTag("Player");
        playerControl = playerControlObj.GetComponent<PlayerControl>();
        playerControl.SkillPressed += OnSkillButtonReceived;
        skillsCoolDown = new float[skillsData.Length];
        for (int i = 0; i < skillsData.Length; i++)
        {
            if (skillsData[i].icon)
            {
                hudSkillScript[i].SetSkillIcon(skillsData[i].icon);
            }
            if (skillsData[i] != null) skillsCoolDown[i] = skillsData[i].coolDown;
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
        if (skillsData[index] == null) return;
        if (skillsCoolDown[index] <= 0)
        {
            UseSkill.Invoke(skillsData[index]);
            // Debug.Log(skillsCoolDown[index]);
            skillsCoolDown[index] = skillsData[index].coolDown;
            hudSkillScript[index].StartTimer(skillsCoolDown[index]);
        }
    }

    void AttackStoped()
    {

    }
}
