using System;
using System.Collections.Generic;

namespace JackSParrot.Utils
{
    public class EventDispatcher
    {
        public delegate void Listener<T>(T e) where T : class;

        readonly Dictionary<Type, List<Delegate>> _listeners = new Dictionary<Type, List<Delegate>>();
        
        public void Clear()
        {
            _listeners.Clear();
        }

        public void AddListener<T>(Listener<T> listener) where T : class
        {
            List<Delegate> d;
            if(!_listeners.TryGetValue(typeof(T), out d))
            {
                d = new List<Delegate>();
                _listeners[typeof(T)] = d;
            }
            if(!d.Contains(listener))
                d.Add(listener);
        }

        public void RemoveListener<T>(Listener<T> listener) where T : class
        {
            List<Delegate> group;
            if(_listeners.TryGetValue(typeof(T), out group))
            {
                group.Remove(listener);
            }
        }

        public void Raise<T>() where T : class, new()
        {
            Raise<T>(new T());
        }

        public void Raise<T>(T e) where T : class
        {
            if(e == null)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("Raised event with a null parameter");
                return;
            }

            List<Delegate> listeners;
            if(!_listeners.TryGetValue(typeof(T), out listeners))
            {
                SharedServices.GetService<ICustomLogger>()?.LogDebug("Raised event with no listeners");
                return;
            }
            listeners = new List<Delegate>(listeners);

            for(int i = 0; i < listeners.Count; ++i)
            {
                var callback = listeners[i] as Listener<T>;

                if(callback != null)
                {
                    callback(e);
                }
            }
        }
    }
}