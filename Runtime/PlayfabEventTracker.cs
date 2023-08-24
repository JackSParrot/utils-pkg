/*
using JackSParrot.Utils;

namespace Game.Services
{
    class PlayfabEventTracker : IEventTracker
    {
        PlayFabService _serviceInternal = null;
        PlayFabService _service
        {
            get
            {
                if (_serviceInternal == null)
                {
                    _serviceInternal = ServiceLocator.GetService<PlayFabService>();
                }
                return _serviceInternal;
            }
        }

        public void Dispose()
        {
            _serviceInternal = null;
        }

        public void TrackEvent(ITrackEvent trackEvent)
        {
            _service.TrackEvent(trackEvent.EventName, trackEvent.Parameters);
        }
    }
}
*/