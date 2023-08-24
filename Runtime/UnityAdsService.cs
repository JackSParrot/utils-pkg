using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using JackSParrot.Services;
using UnityEngine.Advertisements;

namespace JackSParrot.Services
{
	public class UnityAdsService : AService, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
	{
		[SerializeField]
		private string _adsGameId;

		[SerializeField]
		private string _adUnitId;

		[SerializeField]
		private bool _testMode;

		private bool _isAdLoaded = false;

		public bool IsAdReady     => IsInitialized && _isAdLoaded;
		public bool IsInitialized => _initializationTaskStatus == TaskStatus.RanToCompletion;

		private TaskStatus _initializationTaskStatus = TaskStatus.Created;
		private TaskStatus _showTaskStatus           = TaskStatus.Created;

		public override List<Type> GetDependencies()
		{
			return new List<Type>();
		}

		public override IEnumerator Initialize()
		{
			_initializationTaskStatus = TaskStatus.Running;
			Advertisement.Initialize(_adsGameId, _testMode, this);
			while (_initializationTaskStatus == TaskStatus.Running)
			{
				yield return new WaitForSeconds(0.2f);
			}

			Status = EServiceStatus.Initialized;
		}

		public void OnInitializationComplete()
		{
			Debug.Log("Unity Ads initialization complete.");
			LoadAd();
			_initializationTaskStatus = TaskStatus.RanToCompletion;
		}

		public void OnInitializationFailed(UnityAdsInitializationError error, string message)
		{
			Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
			_initializationTaskStatus = TaskStatus.Faulted;
		}

		private void LoadAd()
		{
			_isAdLoaded = false;
			Debug.Log("Loading Ad: " + _adUnitId);
			Advertisement.Load(_adUnitId, this);
		}

		public void OnUnityAdsAdLoaded(string adUnitId)
		{
			_isAdLoaded = true;
			Debug.Log("Ad Loaded: " + adUnitId);
		}

		public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
		{
			Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
			Advertisement.Load(_adUnitId, this);
		}

		public async Task<bool> ShowAd()
		{
			if (_showTaskStatus == TaskStatus.Running)
				return false;

			if (!IsInitialized)
				return false;

			if (!IsAdReady)
				return false;

			_showTaskStatus = TaskStatus.Running;

			Advertisement.Show(_adUnitId, this);
#if UNITY_EDITOR
			await Task.Delay(2000);
			OnUnityAdsShowComplete(_adUnitId, UnityAdsShowCompletionState.COMPLETED);
#endif
			while (_showTaskStatus == TaskStatus.Running)
			{
				await Task.Delay(500);
			}

			return _showTaskStatus == TaskStatus.RanToCompletion;
		}

		public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
		{
			Debug.Log("Unity Ads Rewarded Ad:" + showCompletionState);
			Advertisement.Load(_adUnitId, this);
			_showTaskStatus = showCompletionState == UnityAdsShowCompletionState.COMPLETED
				? TaskStatus.RanToCompletion
				: TaskStatus.Faulted;
		}

		public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
		{
			Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
			Advertisement.Load(_adUnitId, this);
			_showTaskStatus = TaskStatus.Faulted;
		}

		public void OnUnityAdsShowStart(string adUnitId)
		{
			Debug.Log("Started watching an ad");
		}

		public void OnUnityAdsShowClick(string adUnitId)
		{
			Debug.Log("User clicked in the ad");
		}

		public override void Cleanup() { }
	}
}
