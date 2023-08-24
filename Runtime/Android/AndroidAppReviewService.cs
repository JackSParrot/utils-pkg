using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Services.AppReview
{
	[CreateAssetMenu(fileName = "AndroidAppReviewService", menuName = "JackSParrot/Services/AndroidAppReviewService")]
	public class AndroidAppReviewService: AAppReviewService
	{
		private string url;

		private ADeviceInfoService _deviceInfoService = null;

		public override bool CanShowReview()
		{
			return !string.IsNullOrEmpty(url) && _deviceInfoService != null && _deviceInfoService.HasInternetConnectionQuick();
		}

		public override void ShowReview()
		{
			Application.OpenURL(url);
		}

		public override void Cleanup() { }

		public override List<Type> GetDependencies()
		{
			return new List<Type> { typeof(ADeviceInfoService) };
		}

		public override IEnumerator Initialize()
		{
			_deviceInfoService = ServiceLocator.GetService<ADeviceInfoService>();
			url = $"market://details?id={Application.identifier}";
			Status = EServiceStatus.Initialized;
			yield return null;
		}
	}
}
