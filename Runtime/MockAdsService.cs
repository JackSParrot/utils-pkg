﻿using System;
using System.Collections;
using System.Collections.Generic;
using JackSParrot.Services;
using UnityEngine;

namespace Game.Services
{
	[CreateAssetMenu(fileName = "MockAdsService", menuName = "JackSParrot/Services/MockAdsService")]
	public class MockAdsService: AAdsService
	{
		public override void Cleanup() { }
		public override List<Type> GetDependencies()
		{
			return null;
		}

		public override IEnumerator Initialize()
		{
			Status = EServiceStatus.Initialized;
			yield return null;
		}

		public override bool IsAdReady()
		{
			return true;
		}

		public override bool ShowRewardedVideo(Action<bool> onEndCallback)
		{
			onEndCallback(true);
			return true;
		}
	}
}
