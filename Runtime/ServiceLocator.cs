using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Services
{
	[CreateAssetMenu(fileName = "ServiceLocator", menuName = "JackSParrot/Services/ServiceLocator")]
	public class ServiceLocator: ScriptableObject
	{
		[SerializeField]
		private List<AService> _services = new List<AService>();

		private static Dictionary<Type, AService> _registeredServices = new Dictionary<Type, AService>();

		private static ServiceLocator _instanceRef;
		private static ServiceLocator Instance
		{
			get
			{
				if (_instanceRef == null)
				{
					_instanceRef = Resources.Load<ServiceLocator>("ServiceLocator") ??
								   ScriptableObject.CreateInstance<ServiceLocator>();
				}

				return _instanceRef;
			}
		}

		public static float InitializationProgress = 0f;
		public static bool Initialized => InitializationProgress >= 1f;

		internal static IEnumerator Initialize(ServiceLocatorInitializer initializer)
		{
			InitializationProgress = 0f;
			foreach (AService service in Instance._services)
			{
				RegisterService(service);
			}

			for (int i = 0; i < Instance._services.Count; i++)
			{
				AService service = Instance._services[i];
				yield return initializer.StartCoroutine(InitializeService(service, initializer));
				InitializationProgress = i / (float)Instance._services.Count;
			}

			InitializationProgress = 1f;
			initializer.OnDestroyed += UnregisterAll;
		}

		private static IEnumerator InitializeService(AService service, ServiceLocatorInitializer initializer)
		{
			if (service.Status != EServiceStatus.NotInitialized)
				yield break;
			List<Type> dependencies = service.GetDependencies();
			if (dependencies is { Count: > 0 })
			{
				foreach (Type dependencyType in dependencies)
				{
					if (!_registeredServices.TryGetValue(dependencyType, out AService dependency))
					{
						Debug.LogError($"Service {service.GetType().FullName} has a dependency of type {dependencyType.FullName} that is not registered");
						continue;
					}

					if (dependency.Status == EServiceStatus.NotInitialized)
					{
						yield return initializer.StartCoroutine(InitializeService(dependency, initializer));
					}
				}
			}

			yield return initializer.StartCoroutine(service.Initialize());

			if (service.Status != EServiceStatus.Initialized)
				Debug.LogError($"Service {service.GetType().FullName} failed to initialize with status {service.Status.ToString()}");
		}

		public void Cleanup()
		{
			foreach (KeyValuePair<Type, AService> servicePair in _registeredServices)
			{
				servicePair.Value.Cleanup();
			}

			_registeredServices.Clear();
		}

		public static void RegisterService<T>(T service, bool overwrite = false) where T: AService
		{
			Instance.RegisterServiceInternal(service, overwrite);
		}

		public static bool HasService<T>() where T: AService
		{
			return Instance.HasServiceInternal<T>();
		}

		public static T GetService<T>() where T: AService
		{
			return Instance.GetServiceInternal<T>();
		}

		public static void UnregisterService<T>()
		{
			Instance.UnregisterServiceInternal<T>();
		}

		public static void UnregisterAll()
		{
			Instance.UnregisterAllInternal();
		}

		private void RegisterServiceInternal<T>(T service, bool overwrite = false) where T: AService
		{
			Type type = service.GetType();
			if (_registeredServices.ContainsKey(type))
			{
				if (overwrite)
				{
					UnregisterService<T>();
				}
				else
				{
					UnityEngine.Debug.LogError("Tried to add an already existing service to the service locator: " +
											   type.Name);
					return;
				}
			}

			service.ResetStatus();
			_registeredServices.Add(type, service);
		}

		private bool HasServiceInternal<T>() where T: AService
		{
			return _registeredServices.ContainsKey(typeof(T));
		}

		private T GetServiceInternal<T>() where T: AService
		{
			if (!_registeredServices.TryGetValue(typeof(T), out AService service))
			{
				UnityEngine.Debug.LogWarning("Tried to get a non registered service from the service locator: " +
											 typeof(T).Name);
				return default;
			}

			return (T)service;
		}

		private void UnregisterServiceInternal<T>()
		{
			Type type = typeof(T);
			if (_registeredServices.ContainsKey(type))
			{
				try
				{
					_registeredServices[type].Cleanup();
				}
				catch (System.Exception e)
				{
					UnityEngine.Debug.LogException(e);
				}

				_registeredServices.Remove(type);
			}
		}

		private void UnregisterAllInternal()
		{
			foreach (KeyValuePair<Type, AService> service in _registeredServices)
			{
				try
				{
					service.Value.Cleanup();
				}
				catch (System.Exception e)
				{
					UnityEngine.Debug.LogException(e);
				}
			}

			_registeredServices.Clear();
		}
	}
}
