using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JackSParrot.Services
{
	public abstract class ASceneManagementService: AService
	{
		public abstract Scene ActiveScene { get; }
		public abstract void Persist(Component component);
		public abstract void Persist(GameObject objectToPersist);
		public abstract void SetPersistentScene(string sceneAddress);
		public abstract void TransitionToScene(string toSceneAddress, Action callback = null);
		public abstract void RestartGame();

		public abstract void LoadScene(string sceneAddress, bool additive = false, bool setAsActiveScene = true,
									   Action callback = null);

		public abstract void UnloadScene(string sceneAddress, Action callback = null);
	}
}
