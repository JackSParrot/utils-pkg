using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Advertisements;

namespace JackSParrot.Services
{
	[CreateAssetMenu(fileName = "UnityAdsService", menuName = "JackSParrot/Services/UnityAdsService")]
	public class UnityAdsService : AAdsService, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
	{
		[SerializeField]
		private bool _testMode;

		private bool _isAdLoaded = false;

		public override bool IsAdReady() => _isInitialized && _isAdLoaded;

		private bool _isInitialized => _initializationTaskStatus == TaskStatus.RanToCompletion;

		private TaskStatus _initializationTaskStatus = TaskStatus.Created;
		private TaskStatus _showTaskStatus           = TaskStatus.Created;

		public override List<Type> GetDependencies()
		{
			return new List<Type>();
		}

		public override IEnumerator Initialize()
		{
			_initializationTaskStatus = TaskStatus.Running;
			Advertisement.Initialize(AppId, _testMode, this);
			Status = EServiceStatus.Initialized;
			yield break;
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
			Debug.Log("Loading Ad: " + AppId);
			Advertisement.Load(AdUnit, this);
		}

		public void OnUnityAdsAdLoaded(string adUnitId)
		{
			_isAdLoaded = true;
			Debug.Log("Ad Loaded: " + adUnitId);
		}

		public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
		{
			Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
			Advertisement.Load(AdUnit, this);
		}
		
		public override bool ShowRewardedVideo(Action<bool> onEndCallback)
		{
			ShowAd().ContinueWith(t => onEndCallback?.Invoke(t.Result));
			return true;
		}

		public async Task<bool> ShowAd()
		{
			if (_showTaskStatus == TaskStatus.Running)
				return false;

			if (!_isInitialized)
				return false;

			if (!IsAdReady())
				return false;

			_showTaskStatus = TaskStatus.Running;

			Advertisement.Show(AdUnit, this);
#if UNITY_EDITOR
			await Task.Delay(2000);
			OnUnityAdsShowComplete(AdUnit, UnityAdsShowCompletionState.COMPLETED);
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
			Advertisement.Load(AdUnit, this);
			_showTaskStatus = showCompletionState == UnityAdsShowCompletionState.COMPLETED
				? TaskStatus.RanToCompletion
				: TaskStatus.Faulted;
		}

		public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
		{
			Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
			Advertisement.Load(AdUnit, this);
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
