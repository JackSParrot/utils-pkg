using System;
using System.Collections;
using System.Collections.Generic;
using JackSParrot.Utils;
using UnityEngine;

namespace JackSParrot.Services
{
    [CreateAssetMenu(fileName = "GlobalEventBus", menuName = "JackSParrot/Services/GlobalEventBus")]
    public class GlobalEventBus : AService
    {
        private EventBus _eventBus;
        
        public override void Cleanup()
        {
            _eventBus.Cleanup();
            _eventBus = null;
            Status = EServiceStatus.NotInitialized;
        }

        public override List<Type> GetDependencies()
        {
            return null;
        }

        public override IEnumerator Initialize()
        {
            _eventBus = new EventBus();
            Status = EServiceStatus.Initialized;
            yield return null;
        }

        public void AddListener<T>(Action<T> listener) where T: class
        {
            _eventBus.AddListener(listener);
        }
        
        public void RemoveListener<T>(Action<T> listener) where T: class
        {
            _eventBus.RemoveListener(listener);
        }
        
        public void Raise<T>(T evt) where T: class
        {
            _eventBus.Raise(evt);
        }
    }
}