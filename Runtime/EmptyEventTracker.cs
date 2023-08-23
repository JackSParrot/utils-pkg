using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Services
{
    class EmptyEventTracker : AEventTracker
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
            Debug.Log($"Tracked {trackEvent.EventName} -> {trackEvent.Parameters}");
        }
    }
}
