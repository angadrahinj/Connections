using System;
using UnityEngine;

public class SingletonMB<T> : MonoBehaviour where T : SingletonMB<T>
{
    public bool isPersistent;
    public static T Instance;

    public static T GetInstance()
    {
        if (!Instance)
        {
            var obj = new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>();
            Instance = obj;
            if (obj.isPersistent)
            {
                DontDestroyOnLoad(obj); 
            }
        }
        return Instance;
    }

    protected virtual void Awake()
    {
        if (!Instance)
        {
            Instance = gameObject.GetComponent<T>();
            if (isPersistent)
            {
                DontDestroyOnLoad(gameObject); 
            }
        }
        else
        {
            if (Instance != gameObject.GetComponent<T>())
            {
                Destroy(gameObject);
            }
        }
    }
}
