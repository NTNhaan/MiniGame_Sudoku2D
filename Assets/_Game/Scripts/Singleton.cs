using UnityEngine;

namespace Utils
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        public static T Instance => instance;
        [SerializeField] protected bool _dontDestroyOnLoad;
        void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                if (_dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
                CustomAwake();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        protected virtual void CustomAwake() { }
    }
}

