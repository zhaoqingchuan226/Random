using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillList
{
    [HideInInspector]
    public List<Skill> skills_initial = new List<Skill> { Skill.Fist, Skill.Dart, Skill.Fire,Skill.Defend }; //已有的初始招数

    public List<Skill> skills_Combo = new List<Skill>
    {
        Skill.Fist_Heavy,
        Skill.Dart_Connect,
        Skill.Fire_Hard
    }; //已有的连招

    [HideInInspector]
    public Dictionary<List<Skill>, Skill> C_Skills_P_Skill_Dic =new Dictionary<List<Skill>, Skill>(); //已有的出招表

    //添加新技能
    public void AddNewSkill(Skill skill) //新加的技能如果和之前的技能C_Skills一样，就会覆盖之前的技能
    {
        if (skill.skillType == SkillType.Initial)
        {
            skills_initial.Add(skill);
        }
        else if (skill.skillType == SkillType.Combo)
        {
            skills_Combo.Add(skill);
        }
        Update_C_Skills_P_Skill_Dic();

    }

    // public bool IsSkillInList(Skill skill, List<Skill> skills)//判断一个技能是否在某张表中
    // {
    //     bool b = false;
    //     foreach (var skill1 in skills)
    //     {
    //         if (skill.name == skill1.name)
    //         {
    //             b = true;
    //             break;
    //         }
    //     }
    //     return b;
    // }
    public List<Skill> SearchNextSkills() //找寻可以新学习的技能
    {
        List<Skill> skills_currant = new List<Skill>();
        skills_currant.AddRange(skills_initial);
        skills_currant.AddRange(skills_Combo);
        List<Skill> skills_Next = new List<Skill>(); //收集了所有可能出现在未来的技能
        foreach (var skill in skills_currant)
        {
            if (skill.Skills_Next != null)
            {
                foreach (var s in skill.Skills_Next)
                {
                    Skill skill_Next = Skill.SearchSkill(s);

                    //新技能不能出现在要导出的skill_Nexts中且不能出现在已有的技能表中
                    if (
                        !Skill.IsSkillInList(skill_Next, skills_Next)
                        && !Skill.IsSkillInList(skill_Next, skills_currant)
                    )
                    {
                        skills_Next.Add(new Skill(skill_Next));
                    }
                }
            }
        }

        return skills_Next;
    }

    bool JudgeIsC_SkillsSame(List<Skill> s0, List<Skill> s1) //判断两个出招表的子招数是否相同
    {
        bool b = true;
        if (s0.Count == s1.Count)
        {
            for (var i = 0; i < s0.Count; i++)
            {
                if (s0[i].name != s1[i].name)
                {
                    b = false;
                }
            }
        }
        else
        {
            b = false;
        }
        return b;
    }

    void C_Skill_P_Skill(List<Skill> C_Skills, Skill P_Skill) //制定子技能的集合和其对应的父技能
    {
        bool b = false;
        List<Skill> origin_C_Skills = new List<Skill>();
        foreach (var skills in C_Skills_P_Skill_Dic.Keys)
        {
            if (JudgeIsC_SkillsSame(skills, C_Skills))
            {
                origin_C_Skills = skills;
                b = true;
                break;
            }
        }

        if (b)
        {
            C_Skills_P_Skill_Dic.Remove(origin_C_Skills);

        }
        else
        {

        }
        C_Skills_P_Skill_Dic.Add(C_Skills, P_Skill);
    }

    public void Update_C_Skills_P_Skill_Dic() //根据已有的连招,更新技能表
    {
        C_Skills_P_Skill_Dic.Clear();
        foreach (var skill in skills_Combo)
        {
            //
            List<Skill> values = new List<Skill>();
            values.AddRange(C_Skills_P_Skill_Dic.Values);
            if (
                !Skill.IsSkillInList(skill, values) //如果已有的连招中加入了某个新连续技，但是这个连续技还没有出招表
            )
            {
                //那就更新这个连续技的出招表
                switch (skill.name)
                {
                    case "重拳":
                        C_Skill_P_Skill(new List<Skill> { Skill.Fist, Skill.Fist }, Skill.Fist_Heavy);
                        break;
                    case "连镖":
                        C_Skill_P_Skill(new List<Skill> { Skill.Dart, Skill.Dart },Skill.Dart_Connect);
                        break;
                    case "灼烧":
                        C_Skill_P_Skill(new List<Skill> { Skill.Fire, Skill.Fire }, Skill.Fire_Hard);
                        break;
                    case "超重拳":
                        C_Skill_P_Skill( new List<Skill> { Skill.Fist_Heavy, Skill.Fist },Skill.Fist_SuperHeavy);
                        break;
                    case "大镖雨":
                        C_Skill_P_Skill(new List<Skill> { Skill.Dart_Connect, Skill.Dart }, Skill.Dart_Rain);
                        break;
                    case "炎炎功":
                        C_Skill_P_Skill(new List<Skill> { Skill.Fire_Hard, Skill.Fire },Skill.YanYan);
                        break;
                    case "太极拳":
                        C_Skill_P_Skill(new List<Skill> { Skill.Palm, Skill.Fist }, Skill.Taiji);
                        break;
                    case "暴雨梨花":
                        C_Skill_P_Skill(new List<Skill> { Skill.FlyCutter, Skill.Dart }, Skill.Dart_Storm);
                        break;
                    case "毒火":
                        C_Skill_P_Skill(new List<Skill> { Skill.Poison, Skill.Fire },Skill.PoisonalFire);
                        break;
                    case "金钟罩":
                        C_Skill_P_Skill(new List<Skill> { Skill.Defend, Skill.Defend },Skill.Defend_Gold);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // public void AddNewSkillCombo(Skill skill)
    // {
    //     this.skills_Combo.Insert(0, skill);
    //     Update_C_Skills_P_Skill_Dic();
    // }
}
