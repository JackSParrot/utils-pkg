using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace JackSParrot.Services
{
	[CreateAssetMenu(fileName = "SceneManagementService", menuName = "JackSParrot/Services/SceneManagementService")]
	public class SceneManagementService: ASceneManagementService
	{
		public override Scene ActiveScene => SceneManager.GetActiveScene();

		private readonly Dictionary<string, Scene> _scenes = new();

		private Scene           _persistentScene;
		private CoroutineRunner _coroutineRunner;

		public override void Persist(Component component) => Persist(component.gameObject);

		public override void Persist(GameObject objectToPersist)
		{
			if (!_persistentScene.IsValid())
			{
				Debug.LogError($"Before persisting an object you need to set the persistent scene");
				return;
			}

			SceneManager.MoveGameObjectToScene(objectToPersist, _persistentScene);
		}

		public override void SetPersistentScene(string sceneName)
		{
			if (!_scenes.ContainsKey(sceneName))
			{
				Scene activeScene = ActiveScene;
				if (activeScene.IsValid() && sceneName.Equals(activeScene.name))
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

		public override void TransitionToScene(string sceneName, Action callback = null)
		{
			UnloadScene(ActiveScene.name, () => { LoadScene(sceneName, true, true, callback); });
		}

		public override void RestartGame()
		{
			ServiceLocator.UnregisterAll();
			SceneManager.LoadScene(0);
		}

		public override void LoadScene(string sceneName, bool additive = false, bool setActive = true, Action callback = null)
		{
			if (_scenes.ContainsKey(sceneName))
			{
				Debug.LogError($"Tried to load an already loaded scene {sceneName}");
				callback?.Invoke();
				return;
			}

			_coroutineRunner.StartCoroutine(this, LoadSceneCoroutine(sceneName, additive, setActive, callback));
		}

		public override void UnloadScene(string sceneName, Action callback = null)
		{
			if (_persistentScene.IsValid() &&
				_persistentScene.name.Equals(sceneName, StringComparison.InvariantCultureIgnoreCase))
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

			_coroutineRunner.StartCoroutine(this, UnloadSceneCoroutine(sceneName, callback));
		}

		private IEnumerator UnloadSceneCoroutine(string sceneName, Action callback)
		{
			AsyncOperation handler = SceneManager.UnloadSceneAsync(sceneName);
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
		}

		private IEnumerator LoadSceneCoroutine(string sceneName, bool additive, bool setActive, Action callback)
		{
			Scene previousScene = ActiveScene;
			GameObject[] previousSceneObjects = previousScene.GetRootGameObjects();
			AsyncOperation handler =
				SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
			handler.allowSceneActivation = true;
			while (!handler.isDone)
			{
				yield return null;
			}

			while (!SceneManager.GetSceneByName(sceneName).isLoaded)
			{
				yield return null;
			}

			_scenes.Add(sceneName, SceneManager.GetSceneByName(sceneName));
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
				} while (!hasSetActiveScene);

				while (_scenes[sceneName] != SceneManager.GetActiveScene())
				{
					yield return null;
				}
			}

			Scene active = ActiveScene;
			GameObject[] previousSceneObjectsUpdated = previousScene.GetRootGameObjects();
			foreach (GameObject go in previousSceneObjectsUpdated)
			{
				bool found = false;
				for (int i = 0; i < previousSceneObjects.Length && !found; ++i)
				{
					found = previousSceneObjects[i] == go;
				}

				if (!found)
				{
					SceneManager.MoveGameObjectToScene(go, active);
				}
			}

			callback?.Invoke();
		}

		public override void Cleanup()
		{
			_coroutineRunner = null;
			Status = EServiceStatus.NotInitialized;
		}

		public override List<Type> GetDependencies()
		{
			return new List<Type> { typeof(CoroutineRunner) };
		}

		public override IEnumerator Initialize()
		{
			_coroutineRunner = ServiceLocator.GetService<CoroutineRunner>();
			Status = EServiceStatus.Initialized;
			yield return null;
		}
	}
}
