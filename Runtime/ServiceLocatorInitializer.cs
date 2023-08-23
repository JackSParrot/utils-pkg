using System;
using UnityEngine;

namespace JackSParrot.Services
{
	public class ServiceLocatorInitializer: MonoBehaviour
	{
		internal Action OnDestroyed;

		public void OnDestroy()
		{
			OnDestroyed?.Invoke();
		}

		private void Start()
		{
			StartCoroutine(ServiceLocator.Initialize(this));
		}
	}
}
