using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetSkill : MonoBehaviour, IPointerClickHandler
{
    public int id;//是哪个角色的
    public void OnPointerClick(PointerEventData eventData)
    {
        SkillDisplay skillDisplay = this.gameObject.GetComponent<SkillDisplay>();
        if (id == 0 && Mechanism.Instance.phase == Phase.P0_Set && skillDisplay.skill == null&&!skillDisplay.isLocked)
        {
            Press0();
        }
        else if (id == 1 && Mechanism.Instance.phase == Phase.P1_Set && Mechanism.Instance.isAI == false && skillDisplay.skill == null&&!skillDisplay.isLocked)
        {
            Press1();
        }
    }

    public void Press0()
    {
        SkillDisplay skillDisplay = this.gameObject.GetComponent<SkillDisplay>();
        skillDisplay.skill = Mechanism.Instance.playerDatas[0].Store.skill;
        skillDisplay.Show();
        Mechanism.Instance.playerDatas[0].Store.Clear();
        Mechanism.Instance.playerDatas[0].skillDisplays_thisTurn.Add(skillDisplay);
        StartCoroutine(DelayNextPhase(Phase.P1_Generate));
    }
    public void Press1()
    {
        SkillDisplay skillDisplay = this.gameObject.GetComponent<SkillDisplay>();
        skillDisplay.skill = Mechanism.Instance.playerDatas[1].Store.skill;
        skillDisplay.Show();
        Mechanism.Instance.playerDatas[1].Store.Clear();
        Mechanism.Instance.playerDatas[1].skillDisplays_thisTurn.Add(skillDisplay);
        // Mechanism.Instance.EnterPhase(Phase.P0_Generate);
        StartCoroutine(DelayNextPhase(Phase.P0_Generate));
    }

    public IEnumerator DelayNextPhase(Phase phase)
    {
        yield return new WaitForSeconds(0.5f);
        Mechanism.Instance.EnterPhase(phase);
        yield break;
    }

}
