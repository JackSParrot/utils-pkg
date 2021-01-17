using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace JackSParrot.Utils
{
	public class SceneManagementService : IDisposable
	{
		public class SceneEvent { public string SceneName; }
		public class SceneActivatedEvent : SceneEvent { }
		public class SceneLoadedEvent : SceneEvent { }
		public class SceneUnloadedEvent : SceneEvent { }
		public Scene ActiveScene => SceneManager.GetActiveScene();

		private readonly Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();
		private Scene _persistentScene;

		public void Persist(GameObject objectToPersist)
        {
			if(!_persistentScene.IsValid())
			{
				Debug.LogError($"Before persisting an object you need to set the persistent scene");
				return;
			}
			SceneManager.MoveGameObjectToScene(objectToPersist, _persistentScene);
		}

		public void SetPersistentScene(string sceneName)
		{
			if (!_scenes.ContainsKey(sceneName))
			{
				var activeScene = ActiveScene;
				if(activeScene.IsValid() && sceneName.Equals(activeScene.name))
                {
					_scenes.Add(sceneName, activeScene);
                }
				else
				{
					Debug.LogError($"Scene {sceneName} can not be set as persistent, it has not been loaded");
					return;
				}
			}
			_persistentScene = _scenes[sceneName];
		}

		public void TransitionToScene(string toSceneName, Action callback = null)
		{
			UnloadScene(ActiveScene.name, () =>
			{
				LoadScene(toSceneName, true, true, callback);
			});
		}

		public void LoadScene(string sceneName, bool additive = false, bool setActive = true, Action callback = null)
		{
			if (_scenes.ContainsKey(sceneName))
			{
				Debug.LogError($"Tried to load an already loaded scene {sceneName}");
				callback?.Invoke();
				return;
			}
			if (SharedServices.GetService<ICoroutineRunner>() == null)
			{
				SharedServices.RegisterService<ICoroutineRunner>(new CoroutineRunner());
			}
			SharedServices.GetService<ICoroutineRunner>().StartCoroutine(this, LoadSceneCoroutine(sceneName, additive, setActive, callback));
		}

		public void UnloadScene(string sceneName, Action callback = null)
		{
			if (!_persistentScene.IsValid() && _persistentScene.name.Equals(sceneName, StringComparison.InvariantCultureIgnoreCase))
			{
				Debug.LogError($"Tried to unload the persistent scene {sceneName}. Set a different active scene before unloading it");
				callback?.Invoke();
				return;
			}
			if (!_scenes.ContainsKey(sceneName))
			{
				Debug.LogError($"Tried to unload a scene not loaded {sceneName}");
				callback?.Invoke();
				return;
			}
			if (SharedServices.GetService<ICoroutineRunner>() == null)
			{
				SharedServices.RegisterService<ICoroutineRunner>(new CoroutineRunner());
			}
			SharedServices.GetService<ICoroutineRunner>().StartCoroutine(this, UnloadSceneCoroutine(sceneName, callback));
		}

		private IEnumerator UnloadSceneCoroutine(string sceneName, Action callback)
		{
			var handler = SceneManager.UnloadSceneAsync(sceneName);
			while (!handler.isDone)
			{
				yield return null;
			}
			while (SceneManager.GetSceneByName(sceneName).IsValid())
			{
				yield return null;
			}
			_scenes.Remove(sceneName);
			callback?.Invoke();
			SharedServices.GetService<EventDispatcher>().Raise(new SceneUnloadedEvent { SceneName = sceneName });
		}

		private IEnumerator LoadSceneCoroutine(string sceneName, bool additive, bool setActive, Action callback)
		{
			var handler = SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
			while (!handler.isDone)
			{
				yield return null;
			}
			while (!SceneManager.GetSceneByName(sceneName).isLoaded)
			{
				yield return null;
			}
			_scenes.Add(sceneName, SceneManager.GetSceneByName(sceneName));
			SharedServices.GetService<EventDispatcher>().Raise(new SceneLoadedEvent { SceneName = sceneName });
			if (setActive)
			{
				bool hasSetActiveScene;
				do
				{
					try
					{
						hasSetActiveScene = SceneManager.SetActiveScene(_scenes[sceneName]);
					}
					catch (Exception e)
					{
						UnityEngine.Debug.LogException(e);
						break;
					}
					yield return null;
				}
				while (!hasSetActiveScene);
				while (_scenes[sceneName] != SceneManager.GetActiveScene())
				{
					yield return null;
				}
			}
			callback?.Invoke();
			SharedServices.GetService<EventDispatcher>().Raise(new SceneActivatedEvent { SceneName = sceneName });
		}

		public void Dispose()
		{

		}
	}
}
