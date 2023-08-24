namespace JackSParrot.Services.AppReview
{
	public abstract class AAppReviewService : AService
	{
		public abstract bool CanShowReview();
		public abstract void ShowReview();
	}
}
