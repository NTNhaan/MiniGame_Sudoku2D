using UnityEngine;

namespace Utils
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T intance;
        public static T Intance => intance;
        [SerializeField] protected bool _dontDestroyOnLoad;
        void Awake()
        {
            if (intance == null)
            {
                intance = this as T;
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

