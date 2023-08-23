using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Services
{
    [CreateAssetMenu(fileName = "CoroutineRunner", menuName = "JackSParrot/Services/CoroutineRunner")]
    public class CoroutineRunner : AService
    {
        public class Runner : MonoBehaviour
        {
        }

        Runner _runner = null;

        Dictionary<object, List<Coroutine>> _running = new Dictionary<object, List<Coroutine>>();

        public Coroutine StartCoroutine(object sender, IEnumerator coroutine)
        {
            if (sender == null)
            {
                return null;
            }

            Coroutine ret = _runner.StartCoroutine(coroutine);
            if (!_running.ContainsKey(sender))
            {
                _running.Add(sender, new List<Coroutine>());
            }

            _running[sender].Add(ret);
            _runner.StartCoroutine(RunCoroutine(sender, ret));
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

            foreach (Coroutine cor in _running[sender])
            {
                if (cor != null)
                {
                    _runner.StopCoroutine(cor);
                }
            }

            _running.Remove(sender);
        }

        public override void Cleanup()
        {
            _running.Clear();
            UnityEngine.Object.Destroy(_runner.gameObject);
            Status = EServiceStatus.NotInitialized;
        }

        public override List<Type> GetDependencies()
        {
            return null;
        }

        public override IEnumerator Initialize()
        {
            _runner = new GameObject("CoroutineRunner").AddComponent<Runner>();
            UnityEngine.Object.DontDestroyOnLoad(_runner.gameObject);
            Status = EServiceStatus.Initialized;
            yield return null;
        }

        IEnumerator RunCoroutine(object sender, Coroutine coroutine)
        {
            yield return coroutine;
            _running[sender].Remove(coroutine);
        }
    }
}