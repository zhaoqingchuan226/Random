using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoSingleton<AIPlayer>
{
    public float delayTime = 1f;//AI的反应时间
    public void DelayPlay()
    {
        StartCoroutine(P());
    }
    public IEnumerator P()
    {
        yield return new WaitForSeconds(delayTime);
        AIPlay();
        yield break;
    }
    // delegate void AIFuncs();

    // AIFuncs aIFuncs;
    List<int> DisplayNums = new List<int>();//评分最高的playerdata1的位置
    int Top = 0;//最高分
    void AIPlay()//AI的下棋核心逻辑
    {

        Top = 0;
        DisplayNums.Clear();
        //遍历每一个位置，收集评分最高的位置们
        for (int i = 0; i < 9; i++)
        {
            SkillDisplay sd = Mechanism.Instance.playerDatas[1].skillDisplays[i];
            if (sd.skill == null && !sd.isLocked)
            {
                int n = AIEvaluate(sd, Mechanism.Instance.playerDatas[1].Store.skill);//把当前sd的位置信息和当前store的skill信息传入
                if (n == Top)
                {
                    DisplayNums.Add(i);
                }
                else if (n > Top)
                {
                    Top = n;
                    DisplayNums.Clear();
                    DisplayNums.Add(i);
                }
            }
        }


        //还是会出bug
        SkillDisplay sd_choose = Mechanism.Instance.playerDatas[1].skillDisplays[DisplayNums[Random.Range(0, DisplayNums.Count)]];
        sd_choose.gameObject.GetComponent<SetSkill>().Press1();
    }

    int AIEvaluate(SkillDisplay sd_current, Skill skill)// AI对下这个格子给出的分数
    {
        int n = 0;
        List<SkillDisplay> sds_others = new List<SkillDisplay>();
        Mechanism.Instance.playerDatas[1].FindOtherSD(sds_others, sd_current);

        if (sds_others.Count > 0)//消灭对面第一个目标给的分数
        {
            Skill s0 = skill;
            Skill s1 = sds_others[0].skill;
            if (
                s0.typeColor == TypeColor.Red && s1.typeColor == TypeColor.Green ||
                s0.typeColor == TypeColor.Green && s1.typeColor == TypeColor.Blue ||
                s0.typeColor == TypeColor.Blue && s1.typeColor == TypeColor.Red
            )
            {
                n += s1.value * 15;
            }
            else if (s0.typeColor != TypeColor.Gray && s1.typeColor == TypeColor.Gray)
            {
                n += 1 * 15;
            }

            //如果对面有卡克制自己且对面的卡槽还没满
            if (sds_others.Count < 3)
            {
                foreach (var sd in sds_others)
                {
                    if (
              s0.typeColor == TypeColor.Red && sd.skill.typeColor == TypeColor.Blue ||
              s0.typeColor == TypeColor.Green && sd.skill.typeColor == TypeColor.Red ||
              s0.typeColor == TypeColor.Blue && sd.skill.typeColor == TypeColor.Green
                       )
                    {
                        n -= 2;
                    }
                }
            }


        }

        //合并加分
        List<SkillDisplay> sds = new List<SkillDisplay>();
        Mechanism.Instance.playerDatas[1].FindFilledSD(sds, (sd_current.posNum - 1) % 3);
        sd_current.skill = Mechanism.Instance.playerDatas[1].Store.skill;//先填进去
        sds.Add(sd_current);
        if (sds.Count > 0)
        {
            bool b = Mechanism.Instance.playerDatas[1].C_P_Combo_Test(sds, out List<Skill> C_Skills_Effect, out List<SkillDisplay> sds_Effect, out int deltaScoll);
            if (
            b
            )
            {
                n += deltaScoll * 10;
            }

            //填满三个加分
            if (sds.Count - (b ? 1 : 0) == 3)
            {
                n += 20;
            }
            else if (sds.Count - (b ? 1 : 0) == 2)
            {
                n += 10;
            }

        }
        sd_current.skill = null;//最后删除
        sds.Remove(sd_current);



        //保护加分

        if (skill.typeColor != TypeColor.Gray)//如果是有颜色的，往后放加分 并且后面保护东西的价值越高，加分越多
        {
            n += (sd_current.posNum - 1) / 3 * 2;
            if (sds.Count > 0)
            {
                foreach (var sd in sds)
                {
                    if (sd.posNum > sd_current.posNum)
                    {
                        n += sd.skill.value / 2 * 5;
                    }
                }
            }
        }
        else//如果是无颜色的，往前放加分  并且后面保护东西的价值越高，加分越多
        {
            n += (2 - (sd_current.posNum - 1) / 3) * 2;
            if (sds.Count > 0)
            {
                foreach (var sd in sds)
                {
                    if (sd.posNum > sd_current.posNum)
                    {
                        n += sd.skill.value * 5;
                    }
                }
            }
        }


        return n;
    }













}

public class AIData
{
    public string CharacterName;
    public int HP_MAX;

    public int level;//是第几关的boss

    public Dice dice;

    public List<Skill> skills_Combo = new List<Skill>();


    public AIData(string CharacterName1, int HP_MAX1, int level1, Dice dice1, List<Skill> skills_Combo1)
    {
        CharacterName = CharacterName1;
        HP_MAX = HP_MAX1;
        level = level1;
        dice = dice1;
        skills_Combo = skills_Combo1;
    }

    public static List<AIData> aIDatas_All = new List<AIData>
    {
        new AIData("武学票友",10,1,
        new Dice(new Skill[]{Skill.Fist,Skill.Fist,Skill.Dart,Skill.Fire,Skill.Defend,Skill.Defend}),//Dice
        new List<Skill>{Skill.Fist_Heavy,Skill.Dart_Connect,Skill.Fire_Hard}),//出招表

        // new AIData("少年虚竹",10,1,
        // new Dice(new Skill[]{Skill.Defend,Skill.Defend,Skill.Defend,Skill.Defend,Skill.Defend,Skill.Defend}),//Dice
        // new List<Skill>()),//出招表

        new AIData("扫地僧",20,2,
        new Dice(new Skill[]{Skill.Defend,Skill.Defend,Skill.Defend,Skill.Defend,Skill.Defend,Skill.Defend}),//Dice
        new List<Skill>{Skill.Defend_Gold}),//出招表

         new AIData("专业拳师",20,2,
         new Dice(new Skill[]{Skill.Fist,Skill.Fist,Skill.Palm,Skill.Dart,Skill.Fire,Skill.Defend}),//Dice
         new List<Skill>{Skill.Fist_Heavy,Skill.Dart_Connect,Skill.Fire_Hard}),//出招表

         new AIData("暗器大师",20,2,
         new Dice(new Skill[]{Skill.Dart,Skill.Dart,Skill.FlyCutter,Skill.Fist,Skill.Fire,Skill.Defend}),//Dice
         new List<Skill>{Skill.Fist_Heavy,Skill.Dart_Connect,Skill.Fire_Hard}),//出招表

         new AIData("纵火犯",20,2,
         new Dice(new Skill[]{Skill.Fire,Skill.Fire,Skill.Poison,Skill.Fist,Skill.Dart,Skill.Defend}),//Dice
         new List<Skill>{Skill.Fist_Heavy,Skill.Dart_Connect,Skill.Fire_Hard}),//出招表

         new AIData("杨露禅",30,3,
         new Dice(new Skill[]{Skill.Fist,Skill.Fist,Skill.Palm,Skill.FlyCutter,Skill.Poison,Skill.Defend}),//Dice
         new List<Skill>{Skill.Fist_Heavy,Skill.Fist_SuperHeavy,Skill.Fist_Heavy_Penetrate,Skill.Taiji}),//出招表

         new AIData("谢烟客",30,3,
         new Dice(new Skill[]{Skill.Fire,Skill.Fire,Skill.Poison,Skill.Palm,Skill.FlyCutter,Skill.Defend}),//Dice
         new List<Skill>{Skill.Fire_Hard,Skill.Fire_Hard_Ash,Skill.YanYan,Skill.PoisonalFire}),//出招表

         new AIData("李寻欢",30,3,
         new Dice(new Skill[]{Skill.Dart,Skill.Dart,Skill.FlyCutter,Skill.Palm,Skill.Poison,Skill.Defend}),//Dice
         new List<Skill>{Skill.Dart_Connect,Skill.Dart_Connet_Pierced,Skill.Dart_Rain,Skill.Dart_Storm})//出招表
    };
    public static AIData RandomAIData()
    {
        List<AIData> aIDatas_Level = new List<AIData>();//符合当前level的所有aidatas
        foreach (var aIData in aIDatas_All)
        {
            if (aIData.level == Mechanism.Instance.level)
            {
                aIDatas_Level.Add(aIData);
            }
        }
        return aIDatas_Level[Random.Range(0, aIDatas_Level.Count)];
    }
}
