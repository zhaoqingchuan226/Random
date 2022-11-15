using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T:MonoBehaviour 
{
    private static T instance;
    public static T Instance//第一次读的时候，直接去找有T脚本的目标
    {
        get
        {
            if (instance == null)
            { instance = FindObjectOfType<T>();}//返回一个已加载的T的对象
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
    }
}
