using System;

namespace JackSParrot.Services
{
	public abstract class AAdsService : AService
	{
		public string AppId;
		public string AdUnit;

		public abstract bool IsAdReady();
		public abstract bool ShowRewardedVideo(Action<bool> onEndCallback);
	}
}
