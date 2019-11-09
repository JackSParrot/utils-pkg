using System;
using UnityEngine;

namespace JackSParrot.Utils
{
    public class UnityUpdateScheduler : IUpdateScheduler
    {
        class Updater : MonoBehaviour
        {
            static Updater _instance;
            public static Updater Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new GameObject("UpdateRunner").AddComponent<Updater>();
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                    return _instance;
                }
            }
            public event Action<float> OnUpdate;
            void Update()
            {
                OnUpdate?.Invoke(Time.deltaTime);
            }
        }

        public UnityUpdateScheduler()
        {
            Updater.Instance.OnUpdate += Update;
        }

        protected override void Update(float dt)
        {
            base.Update(dt);
        }
    }
}