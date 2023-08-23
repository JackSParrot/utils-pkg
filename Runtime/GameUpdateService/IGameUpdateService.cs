using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Services
{
	public abstract class AGameUpdateService: AService
	{
		public abstract void TryShowUpdate();
	}
}
