using System;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public event Action<RotationSkillData> UseSkill;

    [SerializeField] private RotationSkillData[] skillsData;
    [SerializeField] private SingularSkill[] hudSkillScript;
    [SerializeField] private Axe axe;
    private RotationSkillData currentRotationSkillOnExec;
    private bool rotationOnExec;
    private int rotationSkillOnExecIndex;
    private PlayerControl playerControl;
    private float[] skillsCoolDown;

    void Start()
    {
        GameObject playerControlObj = GameObject.FindGameObjectWithTag("Player");
        playerControl = playerControlObj.GetComponent<PlayerControl>();
        playerControl.SkillPressed += OnSkillButtonReceived;
        skillsCoolDown = new float[skillsData.Length];
        axe.OnAttackStoped += AttackStoped;

        for (int i = 0; i < skillsData.Length; i++)
        {
            if (skillsData[i].icon != null && hudSkillScript[i] != null)
            {
                hudSkillScript[i].SetSkillIcon(skillsData[i].icon);
            }
            if (skillsData[i] != null) skillsCoolDown[i] = skillsData[i].coolDown;
            // Debug.Log(skillsCoolDown[i]);
        }
    }

    void Update()
    {
        for (int i = 0; i < skillsData.Length; i++)
        {
            if (skillsCoolDown[i] >= 0)
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
            currentRotationSkillOnExec = skillsData[index];
            rotationOnExec = true;
            rotationSkillOnExecIndex = index;
            // Debug.Log(skillsCoolDown[index]);
            skillsCoolDown[index] = skillsData[index].coolDown;
        }
    }

    void AttackStoped()
    {
        hudSkillScript[rotationSkillOnExecIndex].StartTimer(skillsCoolDown[rotationSkillOnExecIndex]);
    }
}
