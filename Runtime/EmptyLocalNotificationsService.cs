using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace JackSParrot.Services.Notifications
{
	[CreateAssetMenu(fileName = "EmptyLocalNotificationsService", menuName = "JackSParrot/Services/EmptyLocalNotificationsService")]
	public class EmptyLocalNotificationsService : ALocalNotificationsService
	{
		public override void Cleanup()
		{

		}

		public override List<Type> GetDependencies()
		{
			return null;
		}

		public override IEnumerator Initialize()
		{
			Status = EServiceStatus.Initialized;
			yield return null;
		}

		public override void UnscheduleAllNotifications()
		{
			Debug.Log("UnscheduleAllNotifications");
		}

		public override void UnscheduleNotification(int id)
		{
			Debug.Log($"UnscheduleAllNotification {id}");
		}

		public override void ScheduleNotification(int channelId, string title, string message, float inSecondsFromNow)
		{
			Debug.Log($"ScheduleNotification[{channelId}][{title}]:[{message}] in [{inSecondsFromNow.ToString(CultureInfo.InvariantCulture)}] seconds");
		}

		public override void AskForPermission(Action<bool> callback) => callback?.Invoke(true);
	}
}
