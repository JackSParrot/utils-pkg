using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace JackSParrot.Services
{
	[CreateAssetMenu(fileName = "DeviceInfoService", menuName = "JackSParrot/Services/DeviceInfoService")]
	public class DeviceInfoService: ADeviceInfoService
	{
		private class DeviceInfoListener: MonoBehaviour
		{
			public Action<bool> ApplicationPaused;
			private void OnApplicationPause(bool pauseStatus) => ApplicationPaused(pauseStatus);
		}

		public override string DeviceUId => SystemInfo.deviceUniqueIdentifier;
		public override float BatteryLevel => SystemInfo.batteryLevel;
		public override string DeviceModel => SystemInfo.deviceModel;
		public override string DeviceName => SystemInfo.deviceName;
		public override DeviceType DeviceType
		{
			get
			{
				switch (SystemInfo.deviceType)
				{
					case UnityEngine.DeviceType.Console:
						return DeviceType.Console;

					case UnityEngine.DeviceType.Desktop:
						return DeviceType.Desktop;

					case UnityEngine.DeviceType.Handheld:
						return DeviceType.Handheld;

					default:
						return DeviceType.Unknown;
				}
			}
		}

		private DeviceInfoListener _listener = null;

		public override IEnumerator Initialize()
		{
			_listener = new GameObject("DeviceInfoListener").AddComponent<DeviceInfoListener>();
			Object.DontDestroyOnLoad(_listener.gameObject);
			_listener.ApplicationPaused += ApplicationPaused;
			Application.lowMemory += LowMemory;
			Status = EServiceStatus.Initialized;
			yield return null;
		}

		private void LowMemory()
		{
			OnLowMemoryReceived?.Invoke();
		}

		public override bool HasInternetConnectionQuick()
		{
			return Application.internetReachability != NetworkReachability.NotReachable;
		}

		public override void HasInternetConnectionTrusted(Action<bool> callback)
		{
			UnityWebRequest req = new UnityWebRequest("http://google.com");
			req.timeout = 10;
			req.SendWebRequest().completed += r => { callback(((UnityWebRequestAsyncOperation)r).webRequest.error != null); };
		}

		void ApplicationPaused(bool paused)
		{
			if (paused)
			{
				OnWillGoToBackground?.Invoke();
			}
			else
			{
				OnWillGoToForeground?.Invoke();
			}
		}

		public override void Cleanup()
		{
			if(_listener && _listener.gameObject)
				Object.Destroy(_listener.gameObject);
		}

		public override List<Type> GetDependencies()
		{
			return null;
		}
	}
}
