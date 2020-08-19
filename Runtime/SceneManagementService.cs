using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace JackSParrot.Utils
{
    public class SceneManagementService : IDisposable
    {
        readonly Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();
        Scene _persistantScene;
        Scene _activeScene => SceneManager.GetActiveScene();

        public void SetPersistentScene(string sceneName)
        {
            if (!_scenes.ContainsKey(sceneName))
            {
                throw new Exception($"Scene {sceneName} can not be set as persistant, it has not been loaded");
            }
            _persistantScene = _scenes[sceneName];
        }

        public void LoadScene(string sceneName, bool additive = false, bool setActive = true, Action callback = null)
        {
            if(_scenes.ContainsKey(sceneName))
            {
                callback?.Invoke();
                return;
            }
            if(SharedServices.GetService<ICoroutineRunner>() == null)
            {
                SharedServices.RegisterService<ICoroutineRunner>(new CoroutineRunner());
            }
            SharedServices.GetService<ICoroutineRunner>().StartCoroutine(this, LoadSceneCoroutine(sceneName, additive, setActive, callback));
        }

        public void UnloadScene(string sceneName, Action callback = null)
        {
            if(!_scenes.ContainsKey(sceneName))
            {
                callback?.Invoke();
                return;
            }
            SharedServices.GetService<ICoroutineRunner>().StartCoroutine(this, UnloadSceneCoroutine(sceneName, callback));
        }

        IEnumerator UnloadSceneCoroutine(string sceneName, Action callback)
        {
            SceneManager.UnloadSceneAsync(sceneName);
            yield return null;
            while (SceneManager.GetSceneByName(sceneName).IsValid())
            {
                yield return null;
            }
            yield return null;
            _scenes.Remove(sceneName);
            callback?.Invoke();
            yield return null;
        }

        IEnumerator LoadSceneCoroutine(string sceneName, bool additive, bool setActive, Action callback)
        {
            SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            yield return null;
            while (!SceneManager.GetSceneByName(sceneName).IsValid())
            {
                yield return null;
            }
            yield return null;
            _scenes.Add(sceneName, SceneManager.GetSceneByName(sceneName));
            if (setActive)
            {
                while(!SceneManager.SetActiveScene(_scenes[sceneName]))
                {
                    yield return null;
                }
                while(_scenes[sceneName] != SceneManager.GetActiveScene())
                {
                    yield return null;
                }
            }
            yield return null;
            callback?.Invoke();
        }

        public void Dispose()
        {

        }
    }
}
