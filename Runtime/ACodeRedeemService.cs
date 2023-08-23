using System;

namespace JackSParrot.Services
{
	public abstract class ACodeRedeemService : AService
	{
		public abstract void RedeemCode(string code, Action<string> response);
	}
}
