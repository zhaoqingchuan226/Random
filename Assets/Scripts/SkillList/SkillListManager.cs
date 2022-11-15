using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillListManager : MonoSingleton<SkillListManager>
{
    public GameObject SkillDisplayPrefab_Old;

    [HideInInspector]
    public List<GameObject> SkillDisplays_obj_Old = new List<GameObject>(); //本来就有的
    public Transform OldSkillGroup;

    public GameObject SkillDisplayPrefab;

    [HideInInspector]
    public List<GameObject> SkillDisplays_obj_New = new List<GameObject>(); //三选一生成的
    public Transform NewSkillGroup;
    public GameObject Panel;
    public bool isStudy = false;

    void Reset()
    {
        isStudy = false;

        foreach (var obj in SkillDisplays_obj_New)
        {
            Destroy(obj);
        }
        foreach (var obj in SkillDisplays_obj_Old)
        {
            Destroy(obj);
        }
        SkillDisplays_obj_New.Clear();
        SkillDisplays_obj_Old.Clear();
    }

    public void Open()
    {
        Reset();
        Panel.SetActive(true);
        ShowCurrantSkillDisplays();
        GenerateNewSkillDisplays();
    }

    public void Close()
    {
        Panel.SetActive(false);
        Mechanism.Instance.EnterGameState(GameState.DiceManage);
    }

    void GenerateOneOldSkillDisplay(Skill skill)
    {
        GameObject skillDisplay_obj = Instantiate(SkillDisplayPrefab_Old, OldSkillGroup);
        SkillDisplay skillDisplay = skillDisplay_obj.GetComponent<SkillDisplay>();
        skillDisplay.skill = skill;
        skillDisplay.Show();
        SkillDisplays_obj_Old.Add(skillDisplay_obj);
    }

    public void ShowCurrantSkillDisplays()
    {
        List<Skill> skills = new List<Skill>();
        skills.AddRange(Mechanism.Instance.playerDatas[0].skillList.skills_initial);
        skills.AddRange(Mechanism.Instance.playerDatas[0].skillList.skills_Combo);
        foreach (var skill in skills)
        {
            GenerateOneOldSkillDisplay(skill);
        }
    }

    public void AddCurrantSkillDisplays(Skill skill)
    {
        GenerateOneOldSkillDisplay(skill);
        Mechanism.Instance.playerDatas[0].skillList.AddNewSkill(skill);
    }

    void GenerateNewSkillDisplays() //生成新的三个选项
    {
        List<Skill> skills = new List<Skill>(); //还没有的招
        skills.AddRange(Mechanism.Instance.playerDatas[0].skillList.SearchNextSkills());
        List<string> ss = new List<string>();
        for (var i = 0; i < 3; i++)
        {
            if (skills.Count > 0)
            {
                Skill skill_origin = skills[Random.Range(0, skills.Count)];
                skills.Remove(skill_origin);
                Skill skill = new Skill(skill_origin);
                GameObject skillDisplay_obj = Instantiate(SkillDisplayPrefab, NewSkillGroup);
                SkillDisplay skillDisplay = skillDisplay_obj.GetComponent<SkillDisplay>();
                skillDisplay.skill = skill;
                skillDisplay.Show();
                SkillDisplays_obj_New.Add(skillDisplay_obj);
            }
        }
    }
}
