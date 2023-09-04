using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JackSParrot.JSON;
using Unity.Services.CloudSave;
using UnityEngine;

namespace JackSParrot.Services
{
	[CreateAssetMenu(fileName = "UnityRemoteStorageService", menuName = "Game/Services/UnityRemoteStorageService")]
	public class UnityRemoteStorageService : AStorageService
	{
		public abstract class AKeyClashSolver : ScriptableObject
		{
			public abstract List<string> KeysToCheck { get; }
			public abstract bool         OnClashUseRemote(string key, string localData, string remoteData);
		}

		[SerializeField]
		private AKeyClashSolver _resolver = null;

		private JSONObject _cache = new JSONObject();
		private UnityAuthenticationService _authService;

		public override void Cleanup()
		{
			_cache                   =  new JSONObject();
			Application.focusChanged -= OnApplicationFocusChanged;
		}

		public override List<Type> GetDependencies()
		{
			return new List<Type>{typeof(UnityAuthenticationService), typeof(TimeService)};
		}

		public override IEnumerator Initialize()
		{
			if (!System.IO.File.Exists(Application.persistentDataPath+"/data_cache.json"))
			{
				System.IO.File.WriteAllText(Application.persistentDataPath+"/data_cache.json", "{}");
			}

			string content = System.IO.File.ReadAllText(Application.persistentDataPath+"/data_cache.json");
			try
			{
				_cache = JSON.JSON.LoadString(content);
			}
			catch (Exception e)
			{
				System.IO.File.WriteAllText(Application.persistentDataPath +$"/data_cache_dump{ServiceLocator.GetService<TimeService>().TimestampSeconds}.json", content);
				_cache = new JSONObject();
			}

			Application.focusChanged += OnApplicationFocusChanged;
			
			_authService             =  ServiceLocator.GetService<UnityAuthenticationService>();
			if (_authService.IsSignedIn)
			{
				Task loadTask = LoadRemoteData();
				while (!loadTask.IsCanceled && !loadTask.IsFaulted && !loadTask.IsCompleted)
				{
					yield return null;
				}
			}

			Status = EServiceStatus.Initialized;
		}

		private async Task LoadRemoteData()
		{
			Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync();

			if (!savedData.TryGetValue("data_cache", out string cloudData))
				return;


			JSONObject remote = null;
			try
			{
				remote = JSON.JSON.LoadString(cloudData);
			}
			catch (Exception e)
			{
				System.IO.File.WriteAllText(Application.persistentDataPath +$"/data_cache_remote_dump{ServiceLocator.GetService<TimeService>().TimestampSeconds}.json", cloudData);
				remote = new JSONObject();
			}
			foreach (KeyValuePair<string, JSON.JSON> kvp in remote)
			{
				if (!_cache.Has(kvp.Key))
				{
					_cache[kvp.Key] = kvp.Value;
				}
				else if (_resolver != null && 
						 _resolver.KeysToCheck.Contains(kvp.Key) &&
						 _resolver.OnClashUseRemote(kvp.Key, _cache[kvp.Key].ToString(), kvp.Value.ToString()))
				{
					_cache[kvp.Key] = kvp.Value;
				}
			}
		}

		public override void Flush()
		{
			string contents = _cache.ToString();
			System.IO.File.WriteAllText(Application.persistentDataPath+"/data_cache.json", contents);
		}

		public override void Save<T>(string key, T data)
		{
			_cache[key] = JsonUtility.ToJson(data);
		}

		public override void SaveInt(string key, int data)
		{
			_cache[key] = data;
		}

		public override void SaveString(string key, string data)
		{
			Debug.Log("save "  +key + "->" + data);
			_cache[key] = data;
		}

		public override void SaveFloat(string key, float data)
		{
			_cache[key] = data;
		}

		public override void SaveBool(string key, bool data)
		{
			_cache[key] = data;
		}

		public override T Get<T>(string key, T defaultValue = default)
		{
			return _cache.Has(key) ? JsonUtility.FromJson<T>(_cache[key]) : defaultValue;
		}

		public override int GetInt(string key, int defaultValue = default)
		{
			return _cache.Has(key) ? _cache[key] : defaultValue;
		}

		public override string GetString(string key, string defaultValue = default)
		{
			return _cache.Has(key) ? _cache[key] : defaultValue;
		}

		public override float GetFloat(string key, float defaultValue = default)
		{
			return _cache.Has(key) ? _cache[key] : defaultValue;
		}

		public override bool GetBool(string key, bool defaultValue = default)
		{
			return _cache.Has(key) ? _cache[key] : defaultValue;
		}

		public override bool HasKey(string key)
		{
			return _cache.Has(key);
		}

		private async void OnApplicationFocusChanged(bool hasFocus)
		{
			if (hasFocus || !_authService.IsSignedIn)
				return;

			try
			{
				Dictionary<string, object> cloudSave = new () {["data_cache"]= _cache.ToString()};
				await CloudSaveService.Instance.Data.ForceSaveAsync(cloudSave);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
	}
}
