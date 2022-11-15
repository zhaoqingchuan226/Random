using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillDisplay : MonoBehaviour
{
    public Skill skill = null;
    public TextMeshProUGUI text;
    public int posNum = 0;
    [HideInInspector] public bool isLocked = false;
    public GameObject Lock_Obj;
    [HideInInspector] public int Lock_Times;
    public TextMeshProUGUI Lock_Times_Text;
    public void Show()
    {
        text.text = skill.name + skill.value;
        switch (skill.typeColor)
        {
            case TypeColor.Red:
                text.color = Color.red;
                break;
            case TypeColor.Green:
                text.color = Color.green;
                break;
            case TypeColor.Blue:
                text.color = Color.blue;
                break;
            case TypeColor.Gray:
                text.color = Color.gray;
                break;
            default:
                break;
        }
    }
    public void Clear()
    {
        skill = null;
        text.text = "";
    }
    public void ClearLock()
    {
        isLocked = false;
        Lock_Obj.SetActive(false);
    }


    public void Lock_Test()//玩家操作结束后开始算解锁
    {
        if (isLocked)
        {
            Lock_Times--;
            Lock_Times_Text.text = Lock_Times.ToString();
            if (Lock_Times <= 0)
            {
                isLocked = false;
                Lock_Obj.SetActive(false);
            }
        }
    }

    public void LockSD(int times1)//锁自己
    {
        isLocked = true;
        Lock_Times = times1;
        Lock_Times_Text.text = Lock_Times.ToString();
        Lock_Obj.SetActive(true);
    }

}
