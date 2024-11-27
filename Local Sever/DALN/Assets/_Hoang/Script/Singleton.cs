using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    protected bool shouldNotDestroyOnLoad = true;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<T>();
                if (instance == null)
                {
                    instance = new GameObject(nameof(T)).AddComponent<T>();
                }
            }
            return instance;
        }

    }
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            if (shouldNotDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void SetShouldNotDestroyOnLoad(bool value)
    {
        shouldNotDestroyOnLoad = value;
    }
}

