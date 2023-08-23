using System;
using System.Collections;
using System.Collections.Generic;
using JackSParrot.Services;
using Unity.Services.Core;
using UnityEngine;

namespace JackSparrot.Services
{
	[CreateAssetMenu(fileName = "UnityServicesManagerInitializer", menuName = "JackSParrot/Services/UnityServicesManagerInitializer")]
	public class UnityServicesManagerInitializer: AService
	{
		public override void Cleanup() { }

		public override List<Type> GetDependencies()
		{
			return null;
		}

		public override IEnumerator Initialize()
		{
			UnityServices.InitializeAsync();
			while (UnityServices.State == ServicesInitializationState.Initializing)
			{
				yield return null;
			}

			Status = UnityServices.State == ServicesInitializationState.Initialized ? EServiceStatus.Initialized : EServiceStatus.Failed;
		}
	}
}
