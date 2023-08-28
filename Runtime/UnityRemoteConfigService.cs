using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.RemoteConfig;
using UnityEngine;

namespace JackSParrot.Services
{
	[CreateAssetMenu(fileName = "UnityRemoteConfigService", menuName = "JackSParrot/Services/UnityRemoteConfigService")]
	public class UnityRemoteConfigService: ARemoteConfigService
	{
		[SerializeField]
		private string environmentId = "";

		public override bool IsLocal => _lastResponse.requestOrigin != ConfigOrigin.Remote;

		struct UserAttributes { }

		struct AppAttributes { }

		ConfigResponse _lastResponse;

		void OnConfigResponse(ConfigResponse response)
		{
			Fetching = false;
			_lastResponse = response;
			Status = EServiceStatus.Initialized;
		}

		public override bool GetBool(string name, bool defaultValue = false)
		{
			bool retVal = defaultValue;
			if (Status == EServiceStatus.Initialized && RemoteConfigService.Instance.appConfig.HasKey(name))
			{
				retVal = RemoteConfigService.Instance.appConfig.GetBool(name);
			}

			return retVal;
		}

		public override float GetFloat(string name, float defaultValue = 0)
		{
			float retVal = defaultValue;
			if (RemoteConfigService.Instance.appConfig.HasKey(name))
			{
				retVal = RemoteConfigService.Instance.appConfig.GetFloat(name);
			}

			return retVal;
		}

		public override int GetInt(string name, int defaultValue = 0)
		{
			int retVal = defaultValue;
			if (RemoteConfigService.Instance.appConfig.HasKey(name))
			{
				retVal = RemoteConfigService.Instance.appConfig.GetInt(name);
			}

			return retVal;
		}

		public override string GetString(string name, string defaultValue = "")
		{
			string retVal = defaultValue;
			if (RemoteConfigService.Instance.appConfig.HasKey(name))
			{
				retVal = RemoteConfigService.Instance.appConfig.GetString(name);
			}

			return retVal;
		}

		public override void Cleanup() { }

		public override List<Type> GetDependencies()
		{
			return new List<Type> { typeof(UnityAuthenticationService) };
		}

		public override IEnumerator Initialize()
		{
			RemoteConfigService.Instance.FetchCompleted += OnConfigResponse;
			if (!string.IsNullOrEmpty(environmentId))
				RemoteConfigService.Instance.SetEnvironmentID(environmentId);
			RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());
			while (Status == EServiceStatus.NotInitialized)
			{
				yield return null;
			}
		}
	}
}
