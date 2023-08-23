using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JackSParrot.Services
{
	public enum DeviceType
	{
		Console,
		Desktop,
		Handheld,
		Unknown
	}

	public abstract class ADeviceInfoService: AService
	{
		public UnityEvent OnWillGoToBackground;
		public UnityEvent OnWillGoToForeground;
		public UnityEvent OnLowMemoryReceived;

		public abstract string DeviceUId { get; }
		public abstract float BatteryLevel { get; }
		public abstract string DeviceModel { get; }
		public abstract string DeviceName { get; }
		public abstract DeviceType DeviceType { get; }

		public abstract bool HasInternetConnectionQuick();
		public abstract void HasInternetConnectionTrusted(Action<bool> callback);
	}
}
