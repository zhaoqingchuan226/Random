using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicaCloth;
public class BodyChangeManager : MonoSingleton<BodyChangeManager>
{
    public GameObject feet;
    public GameObject hand;
    public GameObject head;
    public MagicaAvatar ma;
    Dictionary<GameObject, int> g_i_dic = new Dictionary<GameObject, int>();
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AttachPart(feet);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            DetachPart(feet);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AttachPart(hand);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            DetachPart(hand);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            AttachPart(head);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            DetachPart(head);
        }
    }

    void AttachPart(GameObject part)
    {
        if (!g_i_dic.ContainsKey(part))
        {
            g_i_dic.Add(part, ma.AttachAvatarParts(part));
        }
    }

    void DetachPart(GameObject part)
    {
        if (g_i_dic.ContainsKey(part))
        {
            ma.DetachAvatarParts(g_i_dic[part]);
            g_i_dic.Remove(part);
        }
    }
}
