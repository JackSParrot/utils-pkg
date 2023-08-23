using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Services
{
	[CreateAssetMenu(fileName = "AndroidHapticFeedbackService", menuName = "JackSParrot/Services/AndroidHapticFeedbackService")]
	public class AndroidHapticFeedbackService: AService
	{
#if UNITY_ANDROID
		private int               _hapticFeedbackConstantsKey;
		private AndroidJavaObject _unityPlayer = null;

		public bool Play()
		{
			if (Application.platform != RuntimePlatform.Android)
				return false;
			return _unityPlayer.Call<bool>("performHapticFeedback", _hapticFeedbackConstantsKey);
		}

		public override void Cleanup() { }

		public override List<Type> GetDependencies()
		{
			return null;
		}

		public override IEnumerator Initialize()
		{
			if (Application.platform != RuntimePlatform.Android)
				yield break;
			_hapticFeedbackConstantsKey = new AndroidJavaClass("android.view.HapticFeedbackConstants").GetStatic<int>("VIRTUAL_KEY");
			_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer");
		}
#else
        public override void Cleanup(){}
        public override List<Type> GetDependencies() => return null;
        public override IEnumerator Initialize()
        {
            yield return null;
        }
#endif
	}
}
