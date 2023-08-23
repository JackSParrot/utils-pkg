using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Services
{
    public interface IUpdatable
    {
        void UpdateDelta(float deltaSeconds);
    }
    
    [CreateAssetMenu(fileName = "UpdateScheduler", menuName = "JackSParrot/Services/UpdateScheduler")]
    public class UpdateScheduleService : AService
    {
        private class Updater : MonoBehaviour
        {
            public event Action<float> OnUpdate = dt => { };
            private void Update() => OnUpdate(Time.deltaTime);
        }

        private List<IUpdatable> _registeredUpdatables = new List<IUpdatable>();
        private List<IUpdatable> _updatablesToAdd      = new List<IUpdatable>();
        private List<IUpdatable> _updatablesToRemove   = new List<IUpdatable>();
        private Updater          _updater              = null;

        public void ScheduleUpdate(IUpdatable updatable)
        {
            if (!_updatablesToAdd.Contains(updatable) && !_registeredUpdatables.Contains(updatable))
            {
                _updatablesToAdd.Add(updatable);
            }
        }

        public void UnscheduleUpdate(IUpdatable updatable)
        {
            if (!_updatablesToRemove.Contains(updatable))
            {
                _updatablesToRemove.Add(updatable);
            }
        }

        public override void Cleanup()
        {
            if (_updater != null)
            {
                _updater.OnUpdate -= UpdateDelta;
                Destroy(_updater.gameObject);
                _updater = null;
            }

            _registeredUpdatables.Clear();
            _updatablesToAdd.Clear();
            _updatablesToRemove.Clear();
            Status = EServiceStatus.NotInitialized;
        }

        public override List<Type> GetDependencies()
        {
            return new List<Type>();
        }

        public override IEnumerator Initialize()
        {
            _updater = new GameObject("UpdateRunner").AddComponent<Updater>();
            DontDestroyOnLoad(_updater.gameObject);
            _updater.OnUpdate += UpdateDelta;
            Status = EServiceStatus.Initialized;
            yield return null;
        }

        private void UpdateDelta(float dt)
        {
            foreach (IUpdatable current in _updatablesToRemove)
            {
                _registeredUpdatables.Remove(current);
            }

            _updatablesToRemove.Clear();
            foreach (IUpdatable t in _updatablesToAdd)
            {
                _registeredUpdatables.Add(t);
            }

            _updatablesToAdd.Clear();
            foreach (IUpdatable t in _registeredUpdatables)
            {
                t.UpdateDelta(dt);
            }
        }
    }
}