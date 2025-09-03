using System;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public event Action<RotationSkillData> UseSkill;

    [SerializeField] private RotationSkillData[] skillsData;
    [SerializeField] private SingularSkill[] hudSkillScript;
    [SerializeField] private Axe axe;
    private int rotationSkillOnExecIndex;
    private PlayerControl playerControl;
    private float[] skillsCoolDown;
    private bool startCoolDown = false;

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
            if (skillsData[i] != null) skillsCoolDown[i] = 0;
            // Debug.Log(skillsCoolDown[i]);
        }
    }

    void Update()
    {
        if (!startCoolDown) return;
        if (skillsCoolDown[rotationSkillOnExecIndex] >= 0)
        {
            skillsCoolDown[rotationSkillOnExecIndex] -= Time.deltaTime;
        }
        else if (skillsCoolDown[rotationSkillOnExecIndex] <= 0)
        {
            startCoolDown = false;
        }

    }

    void OnSkillButtonReceived(int index)
    {
        if (skillsData[index] == null) return;
        if (skillsCoolDown[index] <= 0 && axe.AxePlayerCanAttack())
        {
            UseSkill.Invoke(skillsData[index]);
            rotationSkillOnExecIndex = index;
            skillsCoolDown[index] = skillsData[index].coolDown;
            Debug.Log(skillsCoolDown[index]);
        }
    }

    void AttackStoped()
    {
        startCoolDown = true;
        hudSkillScript[rotationSkillOnExecIndex].StartTimer(skillsCoolDown[rotationSkillOnExecIndex]);
    }
}
