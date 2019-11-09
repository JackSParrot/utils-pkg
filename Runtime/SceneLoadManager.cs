using System;
using System.Collections;
using System.Collections.Generic;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace JackSParrot.Utils
{
    public class SceneLoaderObserver
    {
        public event Action OnLoaded;
        public event Action OnUnloaded;
        public string SceneName { get; private set; }
        public UnityEngine.SceneManagement.Scene Scene { get; private set; }
        public float Progress { get; private set; }
        public bool Loaded { get; private set; }

        private SceneLoaderObserver() { }

        public bool IsActiveScene()
        {
            return UnitySceneManager.GetActiveScene().buildIndex == Scene.buildIndex;
        }

        public class Handler
        {
            public SceneLoaderObserver Observer;
            public Handler(string sceneName, UnityEngine.SceneManagement.Scene scene)
            {
                Observer = new SceneLoaderObserver
                {
                    Loaded = false,
                    Progress = 0f,
                    SceneName = sceneName,
                    Scene = scene
                };
            }

            public void SetProgress(float progress)
            {
                Observer.Progress = progress;
            }

            public void FinishedLoading()
            {
                Observer.Loaded = true;
                Observer.Progress = 100f;
                Observer.OnLoaded?.Invoke();
            }

            public void FinishedUnloading()
            {
                Observer.Loaded = false;
                Observer.Progress = 0f;
                Observer.OnUnloaded?.Invoke();
            }
        }
    }

    public class SceneLoadManager
    {
        readonly Dictionary<string, SceneLoaderObserver.Handler> _observerHandlers = new Dictionary<string, SceneLoaderObserver.Handler>();

        public SceneLoaderObserver LoadScene(string sceneName, bool additive = false)
        {
            if (_observerHandlers.ContainsKey(sceneName))
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("Trying to load scene: " + sceneName + " that is already loaded or loading");
                return _observerHandlers[sceneName].Observer;
            }
            var observer = new SceneLoaderObserver.Handler(sceneName, UnitySceneManager.GetSceneByName(sceneName));
            _observerHandlers.Add(sceneName, observer);
            SharedServices.GetService<CoroutineRunner>().StartCoroutine(this, SceneLoadCoroutine(sceneName, additive));
            return observer.Observer;
        }

        public SceneLoaderObserver UnloadScene(string sceneName)
        {
            if (!_observerHandlers.ContainsKey(sceneName) || !_observerHandlers[sceneName].Observer.Loaded)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("Trying to unload scene: " + sceneName + " that is not loaded");
                return null;
            }
            SharedServices.GetService<CoroutineRunner>().StartCoroutine(this, SceneUnloadCoroutine(sceneName));
            return _observerHandlers[sceneName].Observer;
        }

        public bool IsSceneLoaded(string sceneName)
        {
            return _observerHandlers.ContainsKey(sceneName) && _observerHandlers[sceneName].Observer.Loaded;
        }

        public void SetActiveScene(string sceneName)
        {
            if (!IsSceneLoaded(sceneName) || _observerHandlers[sceneName].Observer.IsActiveScene())
            {
                return;
            }

            UnitySceneManager.SetActiveScene(_observerHandlers[sceneName].Observer.Scene);
        }

        public string GetActiveScene()
        {
            foreach (var kvp in _observerHandlers)
            {
                if (kvp.Value.Observer.IsActiveScene())
                {
                    return kvp.Key;
                }
            }

            return string.Empty;
        }

        IEnumerator SceneLoadCoroutine(string sceneName, bool additive)
        {
            if (!_observerHandlers.ContainsKey(sceneName) || _observerHandlers[sceneName].Observer.Loaded)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("SceneLoadCoroutine " + sceneName + " is missing the observer");
                yield break;
            }

            var handler = UnitySceneManager.LoadSceneAsync(sceneName,
                additive ? UnityEngine.SceneManagement.LoadSceneMode.Additive : UnityEngine.SceneManagement.LoadSceneMode.Single);
            var observer = _observerHandlers[sceneName];
            while (handler.progress < 0.9f)
            {
                observer.SetProgress(handler.progress * 100f);
                yield return null;
            }
            handler.allowSceneActivation = true;
            while (!handler.isDone)
            {
                observer.SetProgress(handler.progress * 100f);
                yield return null;
            }
            yield return null;
            var scene = UnitySceneManager.GetSceneByName(sceneName);
            UnitySceneManager.SetActiveScene(scene);
            observer.FinishedLoading();
        }

        IEnumerator SceneUnloadCoroutine(string sceneName)
        {
            if (!_observerHandlers.ContainsKey(sceneName) || !_observerHandlers[sceneName].Observer.Loaded)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("SceneUnloadCoroutine " + sceneName + " is missing the observer");
                yield break;
            }

            var handler = UnitySceneManager.UnloadSceneAsync(sceneName);
            var observer = _observerHandlers[sceneName];
            while (!handler.isDone)
            {
                yield return null;
            }
            yield return null;
            observer.FinishedUnloading();
        }
    }
}
