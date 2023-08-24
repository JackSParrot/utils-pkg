#if UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

namespace JackSParrot.Services.Notifications
{
    [CreateAssetMenu(fileName = "LocalAndroidNotifications", menuName = "JackSParrot/Services/LocalAndroidNotifications")]
    public class LocalAndroidNotifications : ALocalNotificationsService
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
            AndroidNotificationCenter.CancelAllNotifications();
        }

        public override void UnscheduleNotification(int id)
        {

        }

        public override void ScheduleNotification(int channelId, string title, string message, float inSecondsFromNow)
        {
            AndroidNotification notification = new AndroidNotification
            {
                Title = title, 
                Text = message, 
                SmallIcon = "icon_1",
                LargeIcon = "icon_0",
                FireTime = DateTime.Now.AddSeconds(inSecondsFromNow)
            };
            string channelIdString = $"channel{channelId.ToString()}";
            AndroidNotificationChannel channel = new AndroidNotificationChannel()
            {
                Id = channelIdString,
                Name = "Default",
                Importance = Importance.Default,
                Description = "Default notifications channel",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
            AndroidNotificationCenter.SendNotification(notification, channelIdString);
        }

        public override void AskForPermission(Action<bool> callback) => callback?.Invoke(true);
    }
}
#endif