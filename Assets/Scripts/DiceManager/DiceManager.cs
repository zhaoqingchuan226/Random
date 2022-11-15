using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoSingleton<DiceManager>
{
    public List<SkillDisplay> skillDisplays = new List<SkillDisplay>();
    public GameObject SkillDisplayPrefab;
    [HideInInspector] public List<GameObject> SkillDisplays_obj = new List<GameObject>();
    [HideInInspector] public SkillDisplay newSkillDisplay_CurrantChoose = null;//现在选中的行为
    public Transform NewSkillGroup;
    public GameObject Panel;
    [HideInInspector] public bool isReplace = false;

    void Reset()
    {
        isReplace = false;
        newSkillDisplay_CurrantChoose = null;
        foreach (var obj in SkillDisplays_obj)
        {
            Destroy(obj);
        }
        SkillDisplays_obj.Clear();
    }
    public void Open()
    {
        Reset();
        Panel.SetActive(true);
        ShowCurrantSkillDisplays();
        GenerateNewSkillDisplays();
    }

    void ShowCurrantSkillDisplays()
    {
        Skill[] skills = Mechanism.Instance.playerDatas[0].dice.skills;
        for (int i = 0; i < skills.Length; i++)
        {
            skillDisplays[i].skill = skills[i];
            skillDisplays[i].Show();
        }

    }
    void GenerateNewSkillDisplays()//生成新的三个选项
    {
        List<Skill> skills_initial = new List<Skill>();//已有的基础招式

        skills_initial.AddRange(Skill.Skills_Nonredundant(Mechanism.Instance.playerDatas[0].skillList.skills_initial));


        for (var i = 0; i < 3; i++)
        {
            if (skills_initial.Count > 0)
            {
                Skill skill_origin = skills_initial[Random.Range(0, skills_initial.Count)];
                Skill skill = new Skill(skill_origin);
                skills_initial.Remove(skill_origin);
                GameObject skillDisplay_obj = Instantiate(SkillDisplayPrefab, NewSkillGroup);
                SkillDisplay skillDisplay = skillDisplay_obj.GetComponent<SkillDisplay>();
                skillDisplay.skill = skill;
                skillDisplay.Show();
                SkillDisplays_obj.Add(skillDisplay_obj);
            }
        }
    }

    public void Close()
    {
        Panel.SetActive(false);
        Mechanism.Instance.EnterGameState(GameState.Fight);
    }
}
