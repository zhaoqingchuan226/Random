using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeColor
{
    Red,
    Green,
    Blue,
    Gray
}

public enum SkillType//技能类型，分为初始技能和Combo
{
    Initial,
    Combo
}

public class Skill
{

    public string name;
    public int value = 0;
    public TypeColor typeColor;
    public List<string> Skills_Next = new List<string>();
    public List<string> Skills_Front = new List<string>();
    public SkillType skillType = SkillType.Initial;

    public Skill(string name1, int value1, TypeColor typeColor1)
    {
        name = name1;
        value = value1;
        typeColor = typeColor1;
    }

    public Skill(Skill skill)
    {
        name = skill.name;
        value = skill.value;
        typeColor = skill.typeColor;
        Skills_Next = skill.Skills_Next;
        Skills_Front = skill.Skills_Front;
        skillType = skill.skillType;
    }

    public Skill(string name1, int value1, TypeColor typeColor1, List<string> Skills_Next1, List<string> Skills_Front1, SkillType skillType1)
    {
        name = name1;
        value = value1;
        typeColor = typeColor1;
        Skills_Next = Skills_Next1;
        Skills_Front = Skills_Front1;
        skillType = skillType1;
    }

    //基础招式
    public static Skill Origin = new Skill("原", 1, TypeColor.Gray, new List<string> { "拳", "镖", "烧", "防" }, null, SkillType.Initial);
    public static Skill Fist = new Skill("拳", 1, TypeColor.Red, new List<string> { "重拳" }, new List<string> { "原" }, SkillType.Initial);
    public static Skill Palm = new Skill("掌", 2, TypeColor.Red, new List<string> { "太极拳" }, new List<string> { "重拳" }, SkillType.Initial);

    public static Skill Dart = new Skill("镖", 1, TypeColor.Green, new List<string> { "连镖" }, new List<string> { "原" }, SkillType.Initial);
    public static Skill FlyCutter = new Skill("飞刀", 2, TypeColor.Green, new List<string> { "连镖.穿透" }, new List<string> { "连镖" }, SkillType.Initial);

    public static Skill Fire = new Skill("烧", 1, TypeColor.Blue, new List<string> { "灼烧" }, new List<string> { "原" }, SkillType.Initial);
    public static Skill Poison = new Skill("毒", 2, TypeColor.Blue, new List<string> { "毒火" }, new List<string> { "灼烧" }, SkillType.Initial);

    public static Skill Defend = new Skill("防", 2, TypeColor.Gray, new List<string> { "金钟罩" }, new List<string> { "原" }, SkillType.Initial);


    //组合技
    public static Skill Fist_Heavy = new Skill("重拳", 4, TypeColor.Red, new List<string> { "超重拳", "重拳.破甲", "掌","防" }, new List<string> { "拳" }, SkillType.Combo);
    public static Skill Fist_SuperHeavy = new Skill("超重拳", 9, TypeColor.Red, null, new List<string> { "重拳" }, SkillType.Combo);
    public static Skill Taiji = new Skill("太极拳", 6, TypeColor.Red, null, new List<string> { "掌" }, SkillType.Combo);
    public static Skill Fist_Heavy_Penetrate = new Skill("重拳.破甲", 4, TypeColor.Red, null, new List<string> { "重拳" }, SkillType.Combo);//被动

    public static Skill Dart_Connect = new Skill("连镖", 4, TypeColor.Green, new List<string> { "大镖雨", "连镖.穿透", "飞刀" }, new List<string> { "镖" }, SkillType.Combo);
    public static Skill Dart_Rain = new Skill("大镖雨", 9, TypeColor.Green, null, new List<string> { "连镖" }, SkillType.Combo);
    public static Skill Dart_Storm = new Skill("暴雨梨花", 6, TypeColor.Green, null, new List<string> { "飞刀" }, SkillType.Combo);
    public static Skill Dart_Connet_Pierced = new Skill("连镖.穿透", 4, TypeColor.Green, null, new List<string> { "连镖" }, SkillType.Combo);

    public static Skill Fire_Hard = new Skill("灼烧", 4, TypeColor.Blue, new List<string> { "炎炎功", "灼烧.灰烬", "毒" }, new List<string> { "烧" }, SkillType.Combo);
    public static Skill YanYan = new Skill("炎炎功", 9, TypeColor.Blue, null, new List<string> { "灼烧" }, SkillType.Combo);
    public static Skill PoisonalFire = new Skill("毒火", 6, TypeColor.Blue, null, new List<string> { "毒" }, SkillType.Combo);
    public static Skill Fire_Hard_Ash = new Skill("灼烧.灰烬", 4, TypeColor.Blue, null, new List<string> { "灼烧" }, SkillType.Combo);

    public static Skill Defend_Gold = new Skill("金钟罩", 4, TypeColor.Gray, null, new List<string> { "拳" }, SkillType.Combo);


    public static List<Skill> skills = new List<Skill>{
        Origin,Fist,Palm,Dart,FlyCutter,Fire,Poison,Defend,Fist_Heavy,Fist_SuperHeavy,Taiji,Fist_Heavy_Penetrate,Dart_Connect,Dart_Rain,Dart_Storm,
        Dart_Connet_Pierced,Fire_Hard,YanYan,PoisonalFire,Fire_Hard_Ash,Defend_Gold
    };

    public static Skill SearchSkill(string s)
    {
        Skill skill_temp = null;

        foreach (var skill in skills)
        {
            if (skill.name == s)
            {
                skill_temp = new Skill(skill);
                break;
            }
        }

        return skill_temp;
    }

    //未写完
    public static bool IsSkillInList(Skill skill, List<Skill> skills)//判断一个技能是否在某张表中
    {
        bool b = false;
        foreach (var skill1 in skills)
        {
            if (skill.name == skill1.name)
            {
                b = true;
                break;
            }

        }
        return b;
    }

    public static List<Skill> Skills_Nonredundant(List<Skill> origin_Skills)//输入一个集合，返回一个非重复的集合
    {
        List<Skill> skills = new List<Skill>();
        foreach (var skill in origin_Skills)
        {
            if (!IsSkillInList(skill, skills))
            {
                skills.Add(skill);
            }
        }
        return skills;
    }
}
