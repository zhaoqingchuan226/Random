using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerData : MonoBehaviour
{
    public int id;
    public TextMeshProUGUI CharacterName;
    public SkillDisplay[] skillDisplays = new SkillDisplay[9];

    public Dice dice = new Dice();
    public SkillDisplay Store;//现在Store中生成的Skill
    // [HideInInspector] public int scoll;
    // public TextMeshProUGUI scoll_text;
    public List<SkillDisplay> skillDisplays_thisTurn = new List<SkillDisplay>();//这回合combo后，最终发生变化的sd //如果没有combo，那就是加入的那个格子
    // public Skill skill_Combo_thisTurn = null;//这回合打出来的连招


    public int HP = 15;
    public int HP_MAX = 15;
    public TextMeshProUGUI HP_text;
    [HideInInspector] public SkillList skillList = new SkillList();
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        skillList.Update_C_Skills_P_Skill_Dic();
        // Defalut_C_Skill_P_Skill();
    }

    public void ResetAll()
    {
        //清除上一把留下的残局
        foreach (var sd in skillDisplays)
        {
            sd.Clear();
            sd.ClearLock();
        }
        Store.Clear();

        //如果是AI，还需要导入AI信息
        if (Mechanism.Instance.isAI && id == 1)
        {
            AIData aIData = AIData.RandomAIData();

            CharacterName.text = aIData.CharacterName;
            HP_MAX = aIData.HP_MAX;
            dice = aIData.dice;
            skillList.skills_Combo = aIData.skills_Combo;
            skillList.Update_C_Skills_P_Skill_Dic();
        }

        //血加满
        skillDisplays_thisTurn.Clear();
        HP = HP_MAX;
        HP_text.text = HP.ToString();
    }

    void PairReduce(List<Skill> C_Skills1, List<Skill> skills1, List<Skill> skills_Effect)//一对一对减少
    {
        bool b = false;
        Skill s0 = null;
        Skill s1 = null;

        foreach (var skill1 in skills1)
        {
            if (C_Skills1[0].name == skill1.name)
            {
                b = true;
                s0 = C_Skills1[0];
                s1 = skill1;
            }
        }

        if (b)//如果配上了，那就减掉一对
        {
            C_Skills1.Remove(s0);
            skills1.Remove(s1);
            skills_Effect.Add(s1);
            if (C_Skills1.Count > 0)//如果出招表的集合还没消耗完，那就递归
            {
                PairReduce(C_Skills1, skills1, skills_Effect);
            }
        }
    }

    //检测有没有combo//导出生效的子技能组，导出生效的sds，导出生效前后的数值差
    public bool C_P_Combo_Test(List<SkillDisplay> sds, out List<Skill> C_Skills_Effect, out List<SkillDisplay> sds_Effect, out int deltaScoll)
    {
        deltaScoll = 0;
        C_Skills_Effect = null;
        sds_Effect = new List<SkillDisplay>();

        List<Skill> skills = new List<Skill>();//玩家目前有的
        Dictionary<Skill, SkillDisplay> skill_sd_dic = new Dictionary<Skill, SkillDisplay>();
        foreach (var sd in sds)
        {
            skills.Add(sd.skill);
            skill_sd_dic.Add(sd.skill, sd);
        }


        foreach (var C_Skills in skillList.C_Skills_P_Skill_Dic.Keys)//将玩家的现有技能和所有的技能表进行对照
        {
            List<Skill> C_Skills1 = new List<Skill>();
            C_Skills1.AddRange(C_Skills);//出招表的拷贝
            List<Skill> skills1 = new List<Skill>();
            skills1.AddRange(skills);//玩家此列技能的拷贝


            List<Skill> skills_Effect = new List<Skill>();

            PairReduce(C_Skills1, skills1, skills_Effect);

            //如果出招表被消光了
            if (C_Skills1.Count == 0)
            {
                C_Skills_Effect = C_Skills;

                deltaScoll = skillList.C_Skills_P_Skill_Dic[C_Skills_Effect].value;
                foreach (var skill in C_Skills)
                {
                    deltaScoll -= skill.value;
                }

                foreach (var skill in skills_Effect)
                {
                    sds_Effect.Add(skill_sd_dic[skill]);
                }


                return true;
            }

        }
        return false;

    }

    SkillDisplay FindTopestSD(List<SkillDisplay> sds)
    {
        SkillDisplay sd = sds[0];
        foreach (var sd1 in sds)
        {
            if (sd1.posNum < sd.posNum)
            {
                sd = sd1;
            }
        }
        return sd;
    }

    void C_P_Combo(List<SkillDisplay> sds)//按照出招表出招，核心函数！！！！
    {
        if (C_P_Combo_Test(sds, out List<Skill> C_Skills_Effect, out List<SkillDisplay> sds_Effect, out int deltaScoll))//out生效的招式，以及需要处理的sds
        {
            //生效！！！！！！！！！
            foreach (var sd in sds_Effect)
            {
                sd.Clear();
            }
            SkillDisplay sd_first = FindTopestSD(sds_Effect);//找到最前面的sd
            sd_first.skill = new Skill(skillList.C_Skills_P_Skill_Dic[C_Skills_Effect]);
            sd_first.Show();
            skillDisplays_thisTurn.Clear();//覆盖之前的输入
            skillDisplays_thisTurn.Add(sd_first);
        }
    }

    public void GenerateCurrentSkill()
    {
        Store.skill = new Skill(dice.skills[Random.Range(0, dice.skills.Length)]);
        Store.Show();
    }


    // public void CalculateScoll()
    // {
    //     int n = 0;
    //     foreach (var sd in skillDisplays)
    //     {
    //         if (sd.skill != null)
    //         {
    //             n += sd.skill.value;
    //         }
    //     }
    //     scoll = n;
    //     scoll_text.text = n.ToString();
    // }

    public int CalculateSDCount()
    {
        int n = 0;
        foreach (var sd in skillDisplays)
        {
            if (sd.skill != null)
            {
                n++;
            }
        }
        return n;
    }
    public void FindOtherSD(List<SkillDisplay> sds_other, SkillDisplay sd_current)//找到对面的所有不为空的sd
    {
        List<SkillDisplay> sds = new List<SkillDisplay>();
        sds.AddRange(skillDisplays);
        int n = sds.IndexOf(sd_current) % 3;//这个n是对面的首个可能会被消灭的sd的序号

        for (int i = n; i < 9; i += 3)
        {
            SkillDisplay sd = Mechanism.Instance.playerDatas[1 - id].skillDisplays[i];
            if (sd.skill != null)
            {
                sds_other.Add(sd);
            }
        }
    }

    public void FindOtherEmptySD(List<SkillDisplay> sds_other, SkillDisplay sd_current)//找到对面空的sd
    {
        List<SkillDisplay> sds = new List<SkillDisplay>();
        sds.AddRange(skillDisplays);
        int n = sds.IndexOf(sd_current) % 3;//这个n是对面的首个可能会被消灭的sd的序号

        for (int i = n; i < 9; i += 3)
        {
            SkillDisplay sd = Mechanism.Instance.playerDatas[1 - id].skillDisplays[i];
            if (sd.skill == null)
            {
                sds_other.Add(sd);
            }
        }
    }



    //消灭对方的函数
    public void Destroy()
    {
        foreach (var sd in skillDisplays_thisTurn)
        {
            List<SkillDisplay> sds_other = new List<SkillDisplay>();
            FindOtherSD(sds_other, sd);//找到对面的所有sd

            if (sds_other.Count > 0)
            {
                if (IsKill(sds_other[0], sd))
                {
                    sds_other[0].Clear();
                    // Mechanism.Instance.playerDatas[1 - id].CalculateScoll();
                }
            }
        }


    }

    //重拳.破甲的效果
    void Fist_Heavy_Penetrate_Destroy(List<SkillDisplay> sds_other)
    {
        for (int i = 0; i < sds_other.Count; i++)
        {
            // if (sds_other[i].skill.typeColor == TypeColor.Gray)
            // {
            if (sds_other[i].skill != null)
            {
                sds_other[i].Clear();
            }
            // }
        }



    }

    //连镖.穿透的效果
    void Dart_Connect_Pierced_Destroy()
    {
        List<SkillDisplay> sds = new List<SkillDisplay>();

        sds.AddRange(Mechanism.Instance.playerDatas[1 - id].skillDisplays);
        for (int i = 0; i < sds.Count; i++)
        {
            if (sds[i].skill != null)
            {
                if (sds[i].skill.typeColor == TypeColor.Blue)
                {
                    sds[i].Clear();
                }
            }
        }
    }

    //灼烧.灰烬的效果,冻结对面的所有空格2回合
    void Fire_Hard_Ash_Lock(List<SkillDisplay> sds_other)
    {

        for (int i = 0; i < sds_other.Count; i++)
        {
            sds_other[i].LockSD(2);
        }

    }

    //特效
    public void SpecialEffects()
    {
        foreach (var sd in skillDisplays_thisTurn)
        {
            if (sd.skill != null)
            {
                List<SkillDisplay> sds_other = new List<SkillDisplay>();
                if (Skill.IsSkillInList(Skill.Fist_Heavy_Penetrate, skillList.skills_Combo) && sd.skill.name == "重拳")
                {

                    FindOtherSD(sds_other, sd);//找到对面的所有sd
                    Fist_Heavy_Penetrate_Destroy(sds_other);
                }
                else if (Skill.IsSkillInList(Skill.Dart_Connet_Pierced, skillList.skills_Combo) && sd.skill.name == "连镖")
                {

                    // FindOtherSD(sds_other, sd);//找到对面的所有sd
                    Dart_Connect_Pierced_Destroy();
                }
                else if (Skill.IsSkillInList(Skill.Fire_Hard_Ash, skillList.skills_Combo) && sd.skill.name == "灼烧")
                {
                    FindOtherEmptySD(sds_other, sd);//找到对面的所有空的sd
                    Fire_Hard_Ash_Lock(sds_other);
                }
            }
        }


    }

    bool IsKill(SkillDisplay sd_other, SkillDisplay sd)//判断此行为是否克制对面
    {
        TypeColor tc0 = sd.skill.typeColor;
        TypeColor tc1 = sd_other.skill.typeColor;

        if (tc0 != TypeColor.Gray && tc1 == TypeColor.Gray)
        {
            sd_other.skill.value--;
            sd_other.Show();
        }


        if (tc0 == TypeColor.Red && tc1 == TypeColor.Green ||
        tc0 == TypeColor.Green && tc1 == TypeColor.Blue ||
        tc0 == TypeColor.Blue && tc1 == TypeColor.Red ||
        tc0 != TypeColor.Gray && tc1 == TypeColor.Gray && sd_other.skill.value == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FindFilledSD(List<SkillDisplay> sds, int i)//找到本列中所有不为空的SD,i是列数
    {
        for (int j = 0; j < 3; j++)
        {
            if (skillDisplays[i + 3 * j].skill != null)
            {
                sds.Add(skillDisplays[i + 3 * j]);
            }
        }
    }

    //自合成的函数
    public void Combine()
    {
        for (int i = 0; i < 3; i++)
        {
            List<SkillDisplay> sds = new List<SkillDisplay>();
            FindFilledSD(sds, i);
            //sds包含了一列中不为空的sd

            //统计具体的技能有几个
            Dictionary<string, int> name_count_dic = new Dictionary<string, int>();
            Dictionary<string, List<SkillDisplay>> name_sds_dic = new Dictionary<string, List<SkillDisplay>>();
            foreach (var sd in sds)
            {
                if (!name_count_dic.ContainsKey(sd.skill.name))
                {
                    name_count_dic.Add(sd.skill.name, 1);
                    name_sds_dic.Add(sd.skill.name, new List<SkillDisplay> { sd });
                }
                else
                {
                    name_count_dic[sd.skill.name]++;
                    name_sds_dic[sd.skill.name].Add(sd);
                }
            }

            C_P_Combo(sds);


        }
    }



    //终结技
    public bool Ultimate()
    {
        bool b = false;
        for (int i = 0; i < 3; i++)
        {
            List<SkillDisplay> sds = new List<SkillDisplay>();
            FindFilledSD(sds, i);

            if (sds.Count == 3)//满了，那就消除此列的技能，并且给对面造成伤害
            {
                int allScoll = 0;
                foreach (var sd in sds)
                {
                    allScoll += sd.skill.value;
                    sd.Clear();
                }
                Mechanism.Instance.playerDatas[1 - id].HPAdjust(-allScoll);
                Mechanism.Instance.StartCoroutine(Mechanism.Instance.UltimateSign(allScoll, id));
                b = true;
            }
        }
        return b;
    }

    public void HPAdjust(int value)
    {
        HP += value;
        HP_text.text = HP.ToString();
        Mechanism.Instance.JudgeWinner();
    }

    public bool isLockedAllSkipThisTurn()//检测所有SD，看是否还有地方可以下棋
    {
        bool b = true;
        foreach (var sd in skillDisplays)
        {
            if (sd.skill == null && sd.isLocked == false)
            {
                b = false;
            }
        }
        return b;
    }
}
