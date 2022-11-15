using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{
    public Skill[] skills = new Skill[] { Skill.Fist, Skill.Fist, Skill.Dart, Skill.Dart, Skill.Fire, Skill.Defend };//,Skill.Defend
                                                                                                                   // public Skill[] skills = new Skill[]{Skill.Fist,Skill.Defend};//,Skill.Dart,Skill.Ice,
    public Dice(Skill[] skills1)
    {
        for (int i = 0; i < skills1.Length; i++)
        {
            skills[i] = skills1[i];
        }
    }
    public Dice()
    {

    }
    public void Replace(Skill skill, int num)
    {
        skills[num] = skill;
    }
}
