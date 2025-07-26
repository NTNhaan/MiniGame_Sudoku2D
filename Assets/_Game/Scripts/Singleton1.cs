using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Singleton<T> : MonoBehaviour where T : Component
{

    private static T instance;
    [SerializeField] private bool dontDestroyOnLoad;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject g = new GameObject("Controller");
                    instance = g.AddComponent<T>();
                    //g.hideFlags = HideFlags.HideInHierarchy;

                }
            }
            return instance;
        }
    }

    void Awake()
    {
        //DontDestroyOnLoad (gameObject);
        if (instance == null)
        {
            instance = this as T;

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        CustomAwake();
    }

    protected virtual void CustomAwake() { }
}
