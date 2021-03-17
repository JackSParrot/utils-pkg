using System;
using System.Collections.Generic;

namespace JackSParrot.Utils
{
    public class EventDispatcher : IDisposable
    {
        private class Listener
        {
            public Type EventType;
            public Delegate EventDelegate;
        }

        readonly Dictionary<Type, List<Delegate>> _listeners = new Dictionary<Type, List<Delegate>>();
        readonly List<Listener> _listenersToAdd = new List<Listener>();
        readonly List<Listener> _listenersToRemove = new List<Listener>();
        readonly List<dynamic> _eventsToRaise = new List<dynamic>();
        private bool _processing = false;

        public void AddListener<T>(Action<T> listener) where T : class
        {
            var evListener = new Listener { EventType = typeof(T), EventDelegate = listener };
            if(_processing)
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
            var evListener = new Listener { EventType = typeof(T), EventDelegate = listener };
            if (_processing)
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
            if(_processing)
            {
                _eventsToRaise.Add(e);
            }
            else
            {
                InternalRaise(e);
                while(_eventsToRaise.Count > 0)
                {
                    InternalRaise(_eventsToRaise[0]);
                    _eventsToRaise.RemoveAt(0);
                }
            }
        }

        void InternalRaise<T>(T e) where T : class
        {
            UnityEngine.Debug.Assert(e != null, "Raised a null event");

            Type type = e.GetType();
            if (!_listeners.TryGetValue(type, out List<Delegate> listeners))
            {
                UnityEngine.Debug.Log("Raised event with no listeners");
                return;
            }

            _processing = true;
            listeners.RemoveAll(e => e == null);
            foreach(Delegate listener in listeners)
            {
                if (listener is Action<T> castedDelegate)
                {
                    castedDelegate(e);
                }
            }
            _processing = false;

            foreach (var listenerToAdd in _listenersToAdd)
            {
                AddListenerInternal(listenerToAdd);
            }
            _listenersToAdd.Clear();

            foreach (var listenerToRemove in _listenersToRemove)
            {
                RemoveListenerInternal(listenerToRemove);
            }
            _listenersToRemove.Clear();
        }

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            _listeners.Clear();
            _listenersToAdd.Clear();
            _listenersToRemove.Clear();
        }

        #region Internals
        private void AddListenerInternal(Listener listener)
        {
            UnityEngine.Debug.Assert(listener != null, "Added a null listener.");
            if (!_listeners.TryGetValue(listener.EventType, out List<Delegate> delegateList))
            {
                delegateList = new List<Delegate>();
                _listeners[listener.EventType] = delegateList;
            }
            UnityEngine.Debug.Assert(delegateList.Find(e => e == listener.EventDelegate) == null, "Added duplicated event listener to the event dispatcher.");
            delegateList.Add(listener.EventDelegate);
        }

        private void RemoveListenerInternal(Listener listener)
        {
            if (listener != null && _listeners.TryGetValue(listener.EventType, out List<Delegate> group))
            {
                group.RemoveAll(e => e == listener.EventDelegate);
            }
        }
        #endregion
    }
}
