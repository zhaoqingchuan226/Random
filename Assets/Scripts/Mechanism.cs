using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameState//游戏状态
{
    Fight,//对战
    SkillListManage,//学习技能
    DiceManage,//调整骰子
}

public enum Phase
{
    P0_Generate,
    P0_Set,
    P0_Ultimate,
    P1_Generate,
    P1_Set,
    P1_Ultimate
}


public class Mechanism : MonoSingleton<Mechanism>
{
    public bool isAI = false;
    public Phase phase;
    public PlayerData[] playerDatas = new PlayerData[2];
    public int TURN_MAX = 15;
    [HideInInspector] public int turn = 0;
    public TextMeshProUGUI text_turn;
    public TextMeshProUGUI text_gameover;
    [HideInInspector] public bool isGameOver = false;//本局游戏是否已经结束
    [HideInInspector] public bool isAllOver = false;//游戏是否已经结束，在胜利时触发
    [HideInInspector] public GameState gameState = GameState.Fight;
    public GameObject LeaveButton;//去下一关的按钮
    public GameObject RestartButton;//重新开始游戏的button

    [HideInInspector] public int level = 0;
    public TextMeshProUGUI level_Text;

    public TextMeshProUGUI ultimateSign;

    private void Start()
    {
        EnterGameState(GameState.Fight);
        EnterPhase(Phase.P0_Generate);
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            DiceManager.Instance.Open();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            DiceManager.Instance.Close();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SkillListManager.Instance.Open();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SkillListManager.Instance.Close();
        }
    }
    void TurnAddAdd()
    {
        turn++;
        text_turn.text = turn.ToString();
    }
    public void JudgeWinner()
    {

        if (playerDatas[0].HP <= 0 && playerDatas[1].HP > 0)
        {
            text_gameover.text = "你输了";
            RestartButton.SetActive(true);
            isGameOver = true;
        }

        else if (playerDatas[1].HP <= 0 && playerDatas[0].HP > 0)
        {
            text_gameover.text = "你获胜了";
            LeaveButton.SetActive(true);
            isGameOver = true;
            playerDatas[0].HP_MAX += 5;//最大生命值增加
        }

        else if (playerDatas[1].HP <= 0 && playerDatas[0].HP <= 0)
        {
            text_gameover.text = "平局";
            LeaveButton.SetActive(true);
            isGameOver = true;
        }
    }

    IEnumerator DelayEnterPhase(Phase phase)
    {
        yield return new WaitForSeconds(0.5f);
        EnterPhase(phase);
        yield break;
    }


    public void EnterPhase(Phase phase1)
    {
        if (isGameOver)
        {
            return;
        }
        ExitPhase(phase);
        phase = phase1;
        switch (phase)
        {
            case Phase.P0_Generate:
                TurnAddAdd();
                playerDatas[0].GenerateCurrentSkill();
                EnterPhase(Phase.P0_Ultimate);
                break;
            case Phase.P1_Generate:
                playerDatas[1].GenerateCurrentSkill();
                EnterPhase(Phase.P1_Ultimate);
                break;
            case Phase.P0_Ultimate:
                if (playerDatas[0].Ultimate())
                {
                    StartCoroutine(DelayEnterPhase(Phase.P0_Set));
                }
                else
                {
                    EnterPhase(Phase.P0_Set);
                }

                break;
            case Phase.P1_Ultimate:
                if (playerDatas[1].Ultimate())
                {
                    StartCoroutine(DelayEnterPhase(Phase.P1_Set));
                }
                else
                {
                    EnterPhase(Phase.P1_Set);
                }
                break;
            case Phase.P0_Set:
                playerDatas[0].skillDisplays_thisTurn.Clear();
                if (playerDatas[0].isLockedAllSkipThisTurn())
                {
                    EnterPhase(Phase.P1_Generate);
                }
                break;
            case Phase.P1_Set:


                playerDatas[1].skillDisplays_thisTurn.Clear();
                if (playerDatas[1].isLockedAllSkipThisTurn())
                {
                    EnterPhase(Phase.P0_Generate);
                }
                if (isAI)
                {
                    AIPlayer.Instance.DelayPlay();
                }
                break;
            default:
                break;
        }
    }



    void ExitPhase(Phase phase1)
    {
        switch (phase1)
        {
            case Phase.P0_Set:
                playerDatas[0].Combine();
                playerDatas[0].Destroy();
                playerDatas[0].SpecialEffects();

                // playerDatas[0].CalculateScoll();
                // playerDatas[1].CalculateScoll();
                // if (playerDatas[0].CalculateSDCount() == 9)
                // {
                //     JudgeWinner();
                // }
                foreach (var sd in playerDatas[0].skillDisplays)
                {
                    sd.Lock_Test();
                }
                break;
            case Phase.P1_Set:
                playerDatas[1].Combine();
                playerDatas[1].Destroy();
                playerDatas[1].SpecialEffects();

                foreach (var sd in playerDatas[1].skillDisplays)
                {
                    sd.Lock_Test();
                }
                // playerDatas[0].CalculateScoll();
                // playerDatas[1].CalculateScoll();
                // if (playerDatas[1].CalculateSDCount() == 9)
                // {
                //     JudgeWinner();
                // }
                break;
            default:
                break;
        }
    }

    public void EnterGameState(GameState gameState1)
    {
        ExitGameState(gameState);
        gameState = gameState1;
        switch (gameState1)
        {
            case GameState.Fight:
                level++;
                level_Text.text = level.ToString();
                ResetAll();//根据level初始化AI
                EnterPhase(Phase.P0_Generate);
                break;
            case GameState.DiceManage:
                DiceManager.Instance.Open();
                break;
            case GameState.SkillListManage:
                SkillListManager.Instance.Open();
                break;
            default:
                break;
        }
    }

    public void EnterGameState_SkillListManager()//给按钮用的
    {
        EnterGameState(GameState.SkillListManage);
    }

    void ExitGameState(GameState gameState1)
    {
        switch (gameState1)
        {
            case GameState.Fight:
                if (level == 3)
                {
                    SceneManager.LoadScene(0);//胜利，重开
                }
                break;
            default:
                break;
        }
    }

    void ResetAll()
    {
        text_gameover.text = null;
        LeaveButton.SetActive(false);
        isGameOver = false;
        turn = 0;
        text_turn.text = turn.ToString();
        playerDatas[0].ResetAll();
        playerDatas[1].ResetAll();
    }

    public void ReStartGame()
    {
        SceneManager.LoadScene(0);
    }

    public IEnumerator UltimateSign(int n, int id)
    {
        ultimateSign.text = playerDatas[id].CharacterName.text + "打出终结技" + n.ToString();
        yield return new WaitForSeconds(2f);
        ultimateSign.text = "";
        yield break;
    }

}
