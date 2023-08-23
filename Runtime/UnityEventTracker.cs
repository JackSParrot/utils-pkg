using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace JackSParrot.Services
{
    [CreateAssetMenu(fileName = "UnityEventTracker", menuName = "JackSParrot/Services/UnityEventTracker")]
    class UnityEventTracker : AEventTracker
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

        public override void TrackEvent(ITrackEvent trackEvent)
        {
            AnalyticsEvent.Custom(trackEvent.EventName, trackEvent.Parameters);
        }
    }
}
