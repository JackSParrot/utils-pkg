using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.PlayerAccounts;
using UnityEngine;

namespace JackSParrot.Services
{
	[CreateAssetMenu(fileName = "UnityAuthenticationService", menuName = "Game/Services/UnityAuthenticationService")]
	public class UnityAuthenticationService : AService
	{
		public bool   IsSignedIn => PlayerPrefs.HasKey("UnityGameAuthenticationService_token");
		public string UserId     => AuthenticationService.Instance.PlayerId;

		public override void Cleanup() { }

		public override List<Type> GetDependencies()
		{
			return new List<Type> { typeof(UnityServicesManagerInitializer) };
		}

		public override IEnumerator Initialize()
		{
			Task<bool> signInTask = AutoSignIn();
			while (!signInTask.IsCanceled && !signInTask.IsFaulted && !signInTask.IsCompleted)
			{
				yield return null;
			}

			Status = signInTask.IsCanceled || signInTask.IsFaulted ? EServiceStatus.Failed : EServiceStatus.Initialized;
		}

		private async Task<bool> AutoSignIn()
		{
			if (AuthenticationService.Instance.SessionTokenExists && await SignInCachedUserAsync())
				return true;

			if (PlayerPrefs.HasKey("UnityGameAuthenticationService_token") && !await TrySignInWithUnity())
			{
				PlayerPrefs.DeleteKey("UnityGameAuthenticationService_token");
				return false;
			}

			return await SignInCachedUserAsync();
		}

		public async Task<bool> SignIn()
		{
			PlayerAccountService.Instance.SignedIn += SignInWithUnity;
			try
			{
				await PlayerAccountService.Instance.StartSignInAsync();
				return true;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return false;
			}
		}

		private async void SignInWithUnity()
		{
				string token = PlayerAccountService.Instance.AccessToken;
			try
			{
				Debug.Log("Signing in with token: " + token);
				await AuthenticationService.Instance.SignInWithUnityAsync(token);
				Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
				PlayerPrefs.SetString("UnityGameAuthenticationService_token", token);
			}
			catch (Exception ex)
			{
				if(ex is AuthenticationException e && e.Message.Contains("already signed in"))
				{
					PlayerPrefs.SetString("UnityGameAuthenticationService_token", token);
					return;
				}
				PlayerPrefs.DeleteKey("UnityGameAuthenticationService_token");
				Debug.LogException(ex);
			}

			PlayerAccountService.Instance.SignedIn -= SignInWithUnity;
		}

		async Task<bool> TrySignInWithUnity()
		{
			string token = PlayerPrefs.GetString("UnityGameAuthenticationService_token");
			if (string.IsNullOrEmpty(token))
				return false;

			Debug.Log("Signing in with token: " + token);
			try
			{
				await AuthenticationService.Instance.SignInWithUnityAsync(token);
				Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);

				return true;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			return false;
		}

		private async void RefreshToken()
		{
			await PlayerAccountService.Instance.RefreshTokenAsync();
		}

		public void SignOut()
		{
			PlayerAccountService.Instance.SignOut();
			AuthenticationService.Instance.SignOut(true);
		}

		public void OpenAccountPortal()
		{
			Application.OpenURL(PlayerAccountSettings.AccountPortalUrl);
		}

		private async Task<bool> SignInCachedUserAsync()
		{
			try
			{
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			return false;
		}
	}
}
