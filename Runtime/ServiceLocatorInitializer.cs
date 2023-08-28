using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JackSParrot.Services
{
	public class ServiceLocatorInitializer: MonoBehaviour
	{
		public UnityEvent OnInitialized;

		private bool _initialized = false;

		private IEnumerator Start()
		{
			if (_initialized)
			{
				OnInitialized?.Invoke();
				yield break;
				
			}
			StartCoroutine(ServiceLocator.Initialize(this));
			while (!ServiceLocator.Initialized)
				yield return null;
			_initialized = true;
			OnInitialized?.Invoke();
		}
	}
}
