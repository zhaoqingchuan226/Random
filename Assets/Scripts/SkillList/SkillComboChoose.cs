using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillComboChoose : MonoBehaviour, IPointerClickHandler
{
    public GameObject Choose;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (SkillListManager.Instance.isStudy == false)
        {
            SkillListManager.Instance.isStudy = true;
            Choose.SetActive(true);
            SkillListManager.Instance.AddCurrantSkillDisplays(this.gameObject.GetComponent<SkillDisplay>().skill);
        }

    }
}
