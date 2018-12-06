using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using System;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace CubeQuest
{
	public class AccountManager : Java.Lang.Object, GoogleApiClient.IOnConnectionFailedListener
	{
		private readonly GoogleApiClient googleClient;

		private readonly MainActivity mainActivity;

		public AccountManager(MainActivity activity)
		{
			mainActivity = activity;
            
			var signInOptions = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultGamesSignIn)
				.Build();

			googleClient = new GoogleApiClient.Builder(activity)
				.EnableAutoManage(activity, this)
				.AddApi(Auth.GOOGLE_SIGN_IN_API, signInOptions)
				.Build();
		}

		public void OnConnectionFailed(ConnectionResult result)
		{
			// TODO
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get intent used to sign in with Google
		/// </summary>
		public Intent GetSignInIntent() => 
			Auth.GoogleSignInApi.GetSignInIntent(googleClient);

		/// <summary>
		/// Handle Intent result
		/// (called from OnActivityResult)
		/// </summary>
		public void HandleResult(Intent data)
		{
			// Result from intent GoogleSignInApi.GetSignInIntent()
			HandleSignInResult(Auth.GoogleSignInApi.GetSignInResultFromIntent(data));
		}

		private void HandleSignInResult(GoogleSignInResult result)
		{
			// Show game if successful
			// TODO

			new AlertDialog.Builder(mainActivity)
				.SetTitle(result.IsSuccess ? "Signed in" : "Sign in error")
				.SetMessage(result.IsSuccess ? 
					"You are now signed in, good for you" : 
					"Couldn't sign you in for some reason")
				.SetPositiveButton("uh", (EventHandler<DialogClickEventArgs>)null)
				.Show();
		}
	}
}