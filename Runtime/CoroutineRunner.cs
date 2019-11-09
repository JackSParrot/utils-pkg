﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Utils
{
    public class CoroutineRunner : IDisposable
    {
        public class Runner : MonoBehaviour {}

        Runner _runner = null;
        Dictionary<object, List<Coroutine>> _running = new Dictionary<object, List<Coroutine>>();
        public CoroutineRunner()
        {
            _runner = new GameObject("CoroutineRunner").AddComponent<Runner>();
            UnityEngine.Object.DontDestroyOnLoad(_runner.gameObject);
        }

        public Coroutine StartCoroutine(object sender, IEnumerator coroutine)
        {
            if(sender == null)
            {
                return null;
            }
            var ret = _runner.StartCoroutine(coroutine);
            if(!_running.ContainsKey(sender))
            {
                _running.Add(sender, new List<Coroutine>());
            }
            _running[sender].Add(ret);
            return ret;
        }

        public void StopCoroutine(object sender, Coroutine coroutine)
        {
            if (sender == null || !_running.ContainsKey(sender) || !_running[sender].Contains(coroutine))
            {
                return;
            }
            _running[sender].Remove(coroutine);
            _runner.StopCoroutine(coroutine);
        }

        public void StopAllCoroutines(object sender)
        {
            if (sender == null || !_running.ContainsKey(sender))
            {
                return;
            }
            foreach(var cor in _running[sender])
            {
                if(cor != null)
                {
                    _runner.StopCoroutine(cor);
                }
            }
            _running.Remove(sender);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(_runner.gameObject);
        }
    }
}