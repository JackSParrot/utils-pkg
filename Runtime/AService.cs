using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Services
{
	public enum EServiceStatus
	{
		NotInitialized,
		Initialized,
		Failed
	}

	public abstract class AService: ScriptableObject
	{
		public EServiceStatus Status { get; protected set; } = EServiceStatus.NotInitialized;
		public abstract void Cleanup();
		public abstract List<Type> GetDependencies();
		public abstract IEnumerator Initialize();

		internal void ResetStatus() => Status = EServiceStatus.NotInitialized;
	}
}
