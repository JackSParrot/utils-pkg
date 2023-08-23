
namespace JackSParrot.Services
{
	public abstract class AEventTracker : AService
	{
		public abstract void TrackEvent(ITrackEvent trackEvent);
	}
}
