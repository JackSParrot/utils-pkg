using System;
using System.Collections.Generic;

namespace JackSParrot.Utils
{
    public interface IUpdatable
    {
        void UpdateDelta(float deltaTime);
    }

    public abstract class IUpdateScheduler
    {
        readonly List<IUpdatable> _registeredUpdatables = new List<IUpdatable>();

        readonly List<IUpdatable> _updatablesToAdd = new List<IUpdatable>();
        readonly List<IUpdatable> _updatablesToRemove = new List<IUpdatable>();

        public void ScheduleUpdate(IUpdatable updatable)
        {
            if(!_updatablesToAdd.Contains(updatable) && !_registeredUpdatables.Contains(updatable))
            {
                _updatablesToAdd.Add(updatable);
            }
        }

        public void UnscheduleUpdate(IUpdatable updatable)
        {
            if(!_updatablesToRemove.Contains(updatable))
            {
                _updatablesToRemove.Add(updatable);
            }
        }

        protected virtual void Update(float dt)
        {
            for(int i = 0; i < _updatablesToRemove.Count; ++i)
            {
                IUpdatable current = _updatablesToRemove[i];
                if(_registeredUpdatables.Contains(current))
                {
                    _registeredUpdatables.Remove(current);
                }
            }
            _updatablesToRemove.Clear();
            for(int i = 0; i < _updatablesToAdd.Count; ++i)
            {
                _registeredUpdatables.Add(_updatablesToAdd[i]);
            }
            _updatablesToAdd.Clear();
            for (int i = 0; i < _registeredUpdatables.Count; ++i)
            {
                _registeredUpdatables[i].UpdateDelta(dt);
            }
        }
    }
}

