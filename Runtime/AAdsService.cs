using System;
using System.Collections.Generic;

namespace JackSParrot.Services
{
	[Serializable]
	public class AdUnit
	{
		public string name;
		public string id;
	}
	
	public enum VideoEndingReason
	{
		Finished,
		Skipped,
		Failed
	}
	
	public abstract class AAdsService: AService
	{
		public string       AppId;
		public List<AdUnit> AdUnits = new List<AdUnit>();

		public abstract bool IsAdReady(string adUnit);
		public abstract bool ShowRewardedVideo(string adUnit, Action<VideoEndingReason> onEndCallback);
	}
}
