using System;
using UnityEngine;
using System.Collections.Generic;

namespace JackSParrot.Utils
{
    public class EventBus
    {
        private class Listener
        {
            public Type     EventType;
            public Delegate EventDelegate;
        }

        private readonly Dictionary<Type, List<Delegate>> _listeners = new Dictionary<Type, List<Delegate>>();

        private readonly List<Listener> _listenersToAdd    = new List<Listener>();
        private readonly List<Listener> _listenersToRemove = new List<Listener>();
        private          int            _processing        = 0;

        public void AddListener<T>(Action<T> listener) where T : class
        {
            Listener evListener = new Listener { EventType = typeof(T), EventDelegate = listener };
            if (_processing > 0)
            {
                _listenersToAdd.Add(evListener);
            }
            else
            {
                AddListenerInternal(evListener);
            }
        }

        public void RemoveListener<T>(Action<T> listener) where T : class
        {
            Listener evListener = new Listener { EventType = typeof(T), EventDelegate = listener };
            if (_processing > 0)
            {
                _listenersToRemove.Add(evListener);
            }
            else
            {
                RemoveListenerInternal(evListener);
            }
        }

        public void Raise<T>() where T : class, new()
        {
            Raise(new T());
        }

        public void Raise<T>(T e) where T : class
        {
#if DEBUG
            if (_processing > 0)
            {
                Debug.LogWarning("Triggered an event while processing a previous event");
            }
#endif

            Debug.Assert(e != null, "Raised a null event");
            Type type = e.GetType();
            if (!_listeners.TryGetValue(type, out List<Delegate> listeners))
            {
                return;
            }

            _processing++;
            listeners.RemoveAll(e => e == null);
            foreach (Delegate listener in listeners)
            {
                if (listener is Action<T> castedDelegate)
                {
                    castedDelegate(e);
                }
            }

            _processing--;

            foreach (Listener listenerToAdd in _listenersToAdd)
            {
                AddListenerInternal(listenerToAdd);
            }

            _listenersToAdd.Clear();

            foreach (Listener listenerToRemove in _listenersToRemove)
            {
                RemoveListenerInternal(listenerToRemove);
            }

            _listenersToRemove.Clear();
        }

        public void Cleanup()
        {
            Clear();
        }

        public void Clear()
        {
            _listeners.Clear();
            _listenersToAdd.Clear();
            _listenersToRemove.Clear();
            _processing = 0;
        }

        private void AddListenerInternal(Listener listener)
        {
            Debug.Assert(listener != null, "Added a null listener.");
            if (!_listeners.TryGetValue(listener.EventType, out List<Delegate> delegateList))
            {
                delegateList = new List<Delegate>();
                _listeners[listener.EventType] = delegateList;
            }

            Debug.Assert(delegateList.Find(e => e == listener.EventDelegate) == null,
                "Added duplicated event listener to the event dispatcher.");
            delegateList.Add(listener.EventDelegate);
        }

        private void RemoveListenerInternal(Listener listener)
        {
            if (listener != null && _listeners.TryGetValue(listener.EventType, out List<Delegate> group))
            {
                group.RemoveAll(e => e == listener.EventDelegate);
            }
        }
    }
}