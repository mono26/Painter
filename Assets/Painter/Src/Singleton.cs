using UnityEngine;

namespace PainterSystem
{
    public class Singleton<T> : SLGBehaviour where T : SLGBehaviour
    {
        public static bool verbose = false;
        public static bool keepAlive = true;

        private static T instance = null;
        public static T Instance
        {
            get {
                if (instance == null) {
                    instance = GameObject.FindObjectOfType<T>();
                    if (instance == null) {
                        GameObject singletonObj = new GameObject();
                        singletonObj.name = typeof(T).ToString();
                        instance = singletonObj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            this.Initialize();
        }

        protected override void OnInitialize()
        {
            if (instance != null) {
                if (verbose) {
                    Debug.Log("SingleAccessPoint, Destroy duplicate instance " + name + " of " + instance.name);
                }
                Destroy(this.gameObject);
                return;
            }

            instance = GetComponent<T>();

            if (keepAlive) {
                DontDestroyOnLoad(this.gameObject);
            }

            if (instance == null) {
                if (verbose) {
                    Debug.LogError("SingleAccessPoint<" + typeof(T).Name + "> Instance null in Awake");
                }
                return;
            }

            if (verbose) {
                Debug.Log("SingleAccessPoint instance found " + instance.GetType().Name);
            }
        }

        public static bool HasInstance()
        {
            return Application.isPlaying && instance != null;
        }
    }
}
