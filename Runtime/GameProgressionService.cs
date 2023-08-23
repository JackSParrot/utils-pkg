using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using JackSParrot.JSON;
using JackSParrot.Services;
using UnityEngine;

namespace JackSParrot.Data
{
	[CreateAssetMenu(fileName = "GameProgressionService", menuName = "JackSParrot/Services/GameProgressionService")]
	public class GameProgressionService: AService
	{
		[SerializeField]
		private ADataPersistanceProvider persistanceProvider = null;
		[SerializeField]
		private AGameProgressionProvider progressionsProvider = null;
		[SerializeField]
		private GameConfigService gameConfigService = null;

		private Dictionary<Type, AGameProgression> _progressions     = new Dictionary<Type, AGameProgression>();
		private List<AGameProgression>             _progressionsList = new List<AGameProgression>();

		private string version = string.Empty;

		public void LockSaves()
		{
			SaveDirtyProgressions();

			_progressionsList.ForEach(p => p.LockSaves());
		}

		public void UnlockSaves()
		{
			_progressionsList.ForEach(p => p.UnlockSaves());

			SaveDirtyProgressions();
		}

		public void Load()
		{
			version = persistanceProvider.Load("version");

			foreach (AGameProgression progression in _progressionsList)
			{
				progression.Load(persistanceProvider.Load(progression.SaveName));
			}

			string appVersion = Application.version;

			MigratePreviousData(appVersion);

			version = appVersion;
			persistanceProvider.Save("version", version);

#if UNITY_EDITOR
			if (PlayerPrefs.HasKey("RESET_PROGRESSION"))
			{
				PlayerPrefs.DeleteKey("RESET_PROGRESSION");
				ResetProgression();
			}
#endif
		}

		private void MigratePreviousData(string appVersion)
		{
			if (string.IsNullOrEmpty(version) || version.Equals(appVersion))
				return;
		}

		public void SaveDirtyProgressions()
		{
			foreach (AGameProgression progression in _progressionsList)
			{
				if (!progression.IsLocked && progression.IsDirty())
				{
					persistanceProvider.Save(progression.SaveName, progression.GetSerializedData());
				}
			}
		}

		public void SaveProgressions()
		{
			CultureInfo culture = CultureInfo.InvariantCulture;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = culture;

			foreach (AGameProgression progression in _progressionsList)
			{
				if (!progression.IsLocked)
				{
					persistanceProvider.Save(progression.SaveName, progression.GetSerializedData());
				}
			}

			persistanceProvider.Flush();

			Thread.CurrentThread.CurrentCulture = currentCulture;
		}

		public string GeneratePlayerSnapshot()
		{
			SaveDirtyProgressions();

			JSONObject obj = new JSONObject { { "version", version } };
			foreach (AGameProgression progression in _progressionsList)
			{
				obj.Add(progression.SaveName, progression.GetSerializedData());
			}

			return obj.ToString();
		}

		public void LoadFromPlayerSnapshot(string data)
		{
			JSONObject otherData = JSON.JSON.LoadString(data);

			persistanceProvider.DeleteAll();

			foreach (string key in otherData.Keys)
			{
				persistanceProvider.Save(key, otherData[key]);
			}

			persistanceProvider.Flush();
		}

		public override void Cleanup()
		{
			_progressions.Clear();
			_progressionsList.Clear();
		}

		public void ResetProgression()
		{
			_progressionsList.ForEach(progression => progression.Reset());
			SaveProgressions();
		}

		public override List<Type> GetDependencies()
		{
			return new List<Type> { typeof(GameConfigService) };
		}

		public override IEnumerator Initialize()
		{
			progressionsProvider.GetProgressions(gameConfigService).ForEach(Add);
			Load();
			Status = EServiceStatus.Initialized;
			yield return null;
		}

		public T Get<T>() where T: AGameProgression
		{
			Debug.Assert(_progressions.ContainsKey(typeof(T)));
			return _progressions[typeof(T)] as T;
		}

		private void Add<T>(T progression) where T: AGameProgression
		{
			Type type = progression.GetType();
			Debug.Assert(!_progressions.ContainsKey(type));
			_progressions.Add(type, progression);
			_progressionsList.Add(progression);
		}
	}
}
