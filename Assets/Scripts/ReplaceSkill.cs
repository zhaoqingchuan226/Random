using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ReplaceSkill : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (DiceManager.Instance.newSkillDisplay_CurrantChoose != null && DiceManager.Instance.isReplace == false)
        {
            DiceManager.Instance.isReplace = true;
            SkillDisplay sd = this.gameObject.GetComponent<SkillDisplay>();
            sd.skill = DiceManager.Instance.newSkillDisplay_CurrantChoose.skill;
            sd.Show();
            int num = sd.posNum - 1;
            Mechanism.Instance.playerDatas[0].dice.Replace(sd.skill, num);
        }
    }
}
