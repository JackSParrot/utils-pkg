using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Services
{
    public abstract class ALocalNotificationsService : AService
    {
        public abstract void AskForPermission(Action<bool> callback);
        public abstract void UnscheduleAllNotifications();
        public abstract void UnscheduleNotification(int id);
        public abstract void ScheduleNotification(int channelId, string title, string message, float inSecondsFromNow);
    }
}