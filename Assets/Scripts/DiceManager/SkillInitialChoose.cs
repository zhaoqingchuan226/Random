using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillInitialChoose : MonoBehaviour, IPointerClickHandler
{
    public GameObject Choose;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (DiceManager.Instance.isReplace == false)
        {
            DiceManager.Instance.newSkillDisplay_CurrantChoose = this.gameObject.GetComponent<SkillDisplay>();

            foreach (var obj in DiceManager.Instance.SkillDisplays_obj)
            {
                SkillInitialChoose skillInitialChoose = obj.GetComponent<SkillInitialChoose>();
                skillInitialChoose.Choose.SetActive(false);
            }
            Choose.SetActive(true);
        }

    }
}
