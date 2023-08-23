using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JackSParrot.Services;
using Unity.Services.Authentication;
using UnityEngine;

namespace JackSparrot.Services
{
	[CreateAssetMenu(fileName = "UnityAuthenticationService", menuName = "JackSParrot/Services/UnityAuthenticationService")]
	public class UnityAuthenticationService: AService
	{
		public override void Cleanup()
		{
			
		}

		public override List<Type> GetDependencies()
		{
			return new List<Type> { typeof(UnityServicesManagerInitializer) };
		}

		public override IEnumerator Initialize()
		{
			if (AuthenticationService.Instance.IsSignedIn)
				yield break;
			Task task = AuthenticationService.Instance.SignInAnonymouslyAsync();
			while(!task.IsCanceled && !task.IsFaulted && !task.IsCompleted)
			{
				yield return null;
			}
			Status = task.IsCanceled || task.IsFaulted ? EServiceStatus.Failed : EServiceStatus.Initialized;
		}
	}
}
