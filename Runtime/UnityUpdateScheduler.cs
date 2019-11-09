using System;
using UnityEngine;

namespace JackSParrot.Utils
{
    public class UnityUpdateScheduler : IUpdateScheduler, IDisposable
    {
        class Updater : MonoBehaviour
        {
            public event Action<float> OnUpdate;
            void Update()
            {
                OnUpdate?.Invoke(Time.deltaTime);
            }
        }
        Updater _updater = null;

        public UnityUpdateScheduler()
        {
            _updater = new GameObject("UpdateRunner").AddComponent<Updater>();
            UnityEngine.Object.DontDestroyOnLoad(_updater.gameObject);
            _updater.OnUpdate += Update;
        }

        protected override void Update(float dt)
        {
            base.Update(dt);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(_updater.gameObject);
        }
    }
}
