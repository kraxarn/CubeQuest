using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Drive;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using Android.Gms.Fitness.Result;
using Android.Gms.Games;
using Android.Gms.Games.Achievement;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Java.Util.Concurrent;

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

		private static Context _context;

	    public static bool IsConnected => _googleClient.IsConnected;

        /// <summary>
        /// Current user signed in
        /// </summary>
        public static User CurrentUser { get; private set; }

        /// <summary>
        /// Google Play display name
        /// </summary>
	    public static string Name => 
	        GamesClass.Players.GetCurrentPlayer(_googleClient).DisplayName;

        /// <summary>
        /// Creates account manager and attempts to sign in silently (triggers <see cref="OnSuccess"/> or <see cref="OnFailure"/>
        /// </summary>
        /// <param name="activity"></param>
	    public static void Create(AppCompatActivity activity)
	    {
            // Ignore if it has already been created
            if (_googleClient != null)
                return;

            // Setup connection listener
            var connectionListener = new ConnectionListener();

            // Save main activity for context and stuffs
	        _context = activity;

            // Setup sign in options
	        var signInOptions = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultGamesSignIn)
	            .RequestScopes(DriveClass.ScopeAppfolder)
	            .RequestScopes(FitnessClass.ScopeActivityRead)
	            .Build();

            // Create google client
	        _googleClient = new GoogleApiClient.Builder(activity)
	            .AddConnectionCallbacks(connectionListener)
	            .AddApi(Auth.GOOGLE_SIGN_IN_API, signInOptions)
	            .AddApi(GamesClass.API)
	            .Build();

            _googleClient.Connect(GoogleApiClient.SignInModeOptional);

            // Wait until we connected and attempt to sign in silently when we do
	        connectionListener.Connected += async hint =>
	        {
                CurrentUser = new User();

	            var silentSignIn = await Auth.GoogleSignInApi.SilentSignIn(_googleClient);

	            if (silentSignIn.Status.IsSuccess)
	                OnSuccess?.Invoke(silentSignIn.Status);
	            else
	                OnFailure?.Invoke(silentSignIn.Status);
	        };

            // Register callback to our connection listener
            _googleClient.RegisterConnectionCallbacks(connectionListener);
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
	        BitmapFactory.DecodeResource(_context.Resources, Resource.Mipmap.ic_launcher_round);

        public static Intent AchievementsIntent => 
            GamesClass.Achievements.GetAchievementsIntent(_googleClient);

        public static void UnlockAchievement(string id) => 
            GamesClass.Achievements.Unlock(_googleClient, id);

        public static void IncrementAchievement(string id, int steps) =>
            GamesClass.Achievements.Increment(_googleClient, id, steps);

        public static async Task<IEnumerable<IAchievement>> GetAchievementsAsync() => 
            (await GamesClass.Achievements.LoadAsync(_googleClient, true)).Achievements;

        public static void SetViewForPopups(View view) => 
            GamesClass.SetViewForPopups(_googleClient, view);

        public static async void GetNumSteps(long start, long end)
        {
            var readRequest = new DataReadRequest.Builder()
                .Aggregate(DataType.TypeStepCountDelta, DataType.AggregateStepCountDelta)
                .SetTimeRange(start, end, TimeUnit.Milliseconds)
                .Build();

            var history = await FitnessClass.HistoryApi.ReadData(_googleClient, readRequest) as DataReadResult;
        }
    }

    public class ConnectionListener : Java.Lang.Object, GoogleApiClient.IOnConnectionFailedListener, GoogleApiClient.IConnectionCallbacks
    {
        public delegate void ConnectionFailedEvent(ConnectionResult result);

        public event ConnectionFailedEvent ConnectionFailed;

        public delegate void ConnectedEvent(Bundle connectionHint);

        public event ConnectedEvent Connected;

        public void OnConnectionFailed(ConnectionResult result) => 
            ConnectionFailed?.Invoke(result);

        public void OnConnected(Bundle connectionHint) => 
            Connected?.Invoke(connectionHint);

        public void OnConnectionSuspended(int cause)
        {
        }
    }
}