﻿using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using System;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace CubeQuest
{
	public static class AccountManager
	{
		private static GoogleApiClient _googleClient;

		private static MainActivity _mainActivity;

	    public static bool IsConnected => _googleClient.IsConnected;

	    public static void Create(MainActivity activity)
	    {
	        _mainActivity = activity;

	        var signInOptions = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultGamesSignIn)
	            .Build();

	        _googleClient = new GoogleApiClient.Builder(activity)
	            .EnableAutoManage(activity, new ConnectionFailedListener())
	            .AddApi(Auth.GOOGLE_SIGN_IN_API, signInOptions)
	            .Build();
	    }

		/// <summary>
		/// Get intent used to sign in with Google
		/// </summary>
		public static Intent GetSignInIntent() => 
			Auth.GoogleSignInApi.GetSignInIntent(_googleClient);

		/// <summary>
		/// Handle Intent result
		/// (called from OnActivityResult)
		/// </summary>
		public static void HandleResult(Intent data)
		{
			// Result from intent GoogleSignInApi.GetSignInIntent()
			HandleSignInResult(Auth.GoogleSignInApi.GetSignInResultFromIntent(data));
		}

		private static void HandleSignInResult(GoogleSignInResult result)
		{
			// Show game if successful
			// TODO

			new AlertDialog.Builder(_mainActivity)
				.SetTitle(result.IsSuccess ? "Signed in" : "Sign in error")
				.SetMessage(result.IsSuccess ? 
					"You are now signed in, good for you" : 
					"Couldn't sign you in for some reason")
				.SetPositiveButton("uh", (EventHandler<DialogClickEventArgs>)null)
				.Show();
		}
	}

    public class ConnectionFailedListener : GoogleApiClient.IOnConnectionFailedListener
    {
        public delegate void ConnectionFailedEvent(ConnectionResult result);

        public event ConnectionFailedEvent ConnectionFailed;

        public void OnConnectionFailed(ConnectionResult result) => 
            ConnectionFailed?.Invoke(result);

        public void Dispose()
        {
        }

        public IntPtr Handle => IntPtr.Zero;
    }
}