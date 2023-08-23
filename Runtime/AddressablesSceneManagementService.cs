using System;
using System.Collections;
using System.Collections.Generic;
using JackSParrot.Utils;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace JackSParrot.Services
{
	[CreateAssetMenu(fileName = "AddressablesSceneManagementService", menuName = "JackSParrot/Services/AddressablesSceneManagementService")]
	public class AddressablesSceneManagementService: ASceneManagementService
	{
		public override Scene ActiveScene => SceneManager.GetActiveScene();

		private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _scenes =
			new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

		private Scene           _persistentScene;
		private CoroutineRunner _coroutineRunner = null;

		public override void Cleanup()
		{
			_scenes.Clear();
			_persistentScene = new Scene();
			_coroutineRunner = null;
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

		public override void SetPersistentScene(string sceneAddress)
		{
			if (!_scenes.TryGetValue(sceneAddress, out AsyncOperationHandle<SceneInstance> operation) || !operation.IsValid() ||
				!operation.Result.Scene.IsValid())
			{
				Debug.LogError($"Scene {sceneAddress} can not be set as persistent, it has not been loaded");
				return;
			}

			_persistentScene = operation.Result.Scene;
		}

		public override void TransitionToScene(string toSceneAddress, Action callback = null)
		{
			UnloadScene(ActiveScene.name, () => { LoadScene(toSceneAddress, true, true, callback); });
		}

		public override void RestartGame()
		{
			ServiceLocator.UnregisterAll();
			SceneManager.LoadScene(0);
		}

		public override void LoadScene(string sceneAddress, bool additive = false, bool setAsActiveScene = true,
									   Action callback = null)
		{
			if (_scenes.ContainsKey(sceneAddress))
			{
				Debug.LogError($"Tried to load an already loaded scene {sceneAddress}");
				callback?.Invoke();
				return;
			}

			_coroutineRunner.StartCoroutine(this,
											additive
												? LoadSceneAdditiveCoroutine(sceneAddress, setAsActiveScene, callback)
												: LoadSceneCoroutine(sceneAddress, callback));
		}

		public override void UnloadScene(string sceneAddress, Action callback = null)
		{
			if (_persistentScene.IsValid() && _persistentScene.name.Equals(sceneAddress))
			{
				Debug.LogError($"Tried to unload the persistent scene {sceneAddress}.");
				callback?.Invoke();
				return;
			}

			if (!_scenes.ContainsKey(sceneAddress))
			{
				Debug.LogError($"Tried to unload a scene not loaded {sceneAddress}");
				callback?.Invoke();
				return;
			}

			_coroutineRunner.StartCoroutine(this, UnloadSceneCoroutine(sceneAddress, callback));
		}

		private IEnumerator UnloadSceneCoroutine(string sceneAddress, Action callback = null)
		{
			AsyncOperation handler = SceneManager.UnloadSceneAsync(sceneAddress);
			while (!handler.isDone)
			{
				yield return null;
			}

			while (SceneManager.GetSceneByName(sceneAddress).IsValid())
			{
				yield return null;
			}

			_scenes.Remove(sceneAddress);
			callback?.Invoke();
		}

		private IEnumerator LoadSceneAdditiveCoroutine(string sceneAddress, bool setAsActiveScene, Action callback)
		{
			List<GameObject> prevRootObjects = null;
			Scene prevActiveScene = ActiveScene;
			if (setAsActiveScene)
			{
				prevRootObjects = new List<GameObject>(prevActiveScene.GetRootGameObjects());
			}

			AsyncOperationHandle<SceneInstance> handler =
				UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Additive, false);
			while (!handler.IsDone)
			{
				yield return null;
			}

			_scenes.Add(sceneAddress, handler);

			AsyncOperation activationHandler = handler.Result.ActivateAsync();
			while (!activationHandler.isDone)
			{
				yield return null;
			}

			if (setAsActiveScene)
			{
				bool hasSetActiveScene;
				do
				{
					try
					{
						hasSetActiveScene = SceneManager.SetActiveScene(handler.Result.Scene) ||
											handler.Result.Scene == SceneManager.GetActiveScene();
					}
					catch (Exception e)
					{
						Debug.LogException(e);
						break;
					}

					if (!hasSetActiveScene)
					{
						yield return null;
					}
				} while (!hasSetActiveScene);

				while (handler.Result.Scene != SceneManager.GetActiveScene())
				{
					yield return null;
				}

				GameObject[] newRootObjects = prevActiveScene.GetRootGameObjects();
				foreach (GameObject go in newRootObjects)
				{
					if (!prevRootObjects.Contains(go))
					{
						SceneManager.MoveGameObjectToScene(go, handler.Result.Scene);
					}
				}
			}

			callback?.Invoke();
		}

		private IEnumerator LoadSceneCoroutine(string sceneAddress, Action callback)
		{
			if (_persistentScene.IsValid())
			{
				_coroutineRunner.StartCoroutine(this, LoadSceneWithPersistant(sceneAddress, callback));
				yield break;
			}

			AsyncOperationHandle<SceneInstance> handler =
				UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Single, false);

			while (!handler.IsDone)
			{
				yield return null;
			}

			_scenes.Add(sceneAddress, handler);

			AsyncOperation activationHandler = handler.Result.ActivateAsync();
			while (!activationHandler.isDone)
			{
				yield return null;
			}

			callback?.Invoke();
		}

		private IEnumerator LoadSceneWithPersistant(string sceneAddress, Action callback)
		{
			List<string> toRemove = new List<string>();
			foreach (KeyValuePair<string, AsyncOperationHandle<SceneInstance>> kvp in _scenes)
			{
				if (kvp.Value.Result.Scene != _persistentScene)
				{
					toRemove.Add(kvp.Key);
				}
			}

			foreach (string addressToRemove in toRemove)
			{
				yield return _coroutineRunner.StartCoroutine(this, UnloadSceneCoroutine(addressToRemove));
			}

			_coroutineRunner.StartCoroutine(this, LoadSceneAdditiveCoroutine(sceneAddress, true, callback));
		}
	}
}
