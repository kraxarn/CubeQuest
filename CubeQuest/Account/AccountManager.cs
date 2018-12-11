using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Drive;
using Android.Gms.Games;
using Android.Graphics;

namespace CubeQuest.Account
{
    public static class AccountManager
    {
        public delegate void SuccessEvent(Statuses status);

        /// <summary>
        /// Login was successful
        /// </summary>
        public static event SuccessEvent OnSuccess;

        public delegate void FailureEvent(Statuses status);

        /// <summary>
        /// Login failed or canceled
        /// </summary>
        public static event FailureEvent OnFailure;

		private static GoogleApiClient _googleClient;

		private static MainActivity _mainActivity;

	    public static bool IsConnected => _googleClient.IsConnected;

        /// <summary>
        /// Google Play display name
        /// </summary>
	    public static string Name => 
	        GamesClass.Players.GetCurrentPlayer(_googleClient).DisplayName;

	    public static void Create(MainActivity activity)
	    {
	        _mainActivity = activity;

	        var signInOptions = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultGamesSignIn)
	            .RequestScopes(DriveClass.ScopeAppfolder)
	            .Build();

	        _googleClient = new GoogleApiClient.Builder(activity)
	            .EnableAutoManage(activity, new ConnectionFailedListener())
	            .AddApi(Auth.GOOGLE_SIGN_IN_API, signInOptions)
	            .AddApi(GamesClass.API)
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
            if (result.IsSuccess)
                OnSuccess?.Invoke(result.Status);
            else if (!result.IsSuccess)
                OnFailure?.Invoke(result.Status);
		}

	    public static Bitmap SaveIcon =>
	        BitmapFactory.DecodeResource(_mainActivity.Resources, Resource.Mipmap.ic_launcher_round);
    }

    public class ConnectionFailedListener : Java.Lang.Object, GoogleApiClient.IOnConnectionFailedListener
    {
        public delegate void ConnectionFailedEvent(ConnectionResult result);

        public event ConnectionFailedEvent ConnectionFailed;

        public void OnConnectionFailed(ConnectionResult result) => 
            ConnectionFailed?.Invoke(result);
    }
}