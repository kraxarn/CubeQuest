using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Drive;
using Android.Gms.Games;
using Android.Gms.Games.Achievement;
using Android.Support.V7.App;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;

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

		private static GoogleApiClient googleClient;

        private static GoogleSignInOptions signInOptions;

		private static Context context;

	    public static bool IsConnected => googleClient.IsConnected;

	    /// <summary>
	    /// Current user signed in
	    /// </summary>
	    public static User CurrentUser { private set; get; }

	    /// <summary>
        /// Google Play display name
        /// </summary>
	    public static string Name => 
	        GamesClass.Players.GetCurrentPlayer(googleClient).DisplayName;

        private static SnapshotManager snapshotManager;

        public static Intent SelectSaveIntent => 
	        snapshotManager.SelectSnapshotIntent;

        /// <summary>
        /// Creates account manager and attempts to sign in silently (triggers <see cref="OnSuccess"/> or <see cref="OnFailure"/>
        /// </summary>
        /// <param name="activity"></param>
	    public static void Create(AppCompatActivity activity)
	    {
            // Ignore if it has already been created
            if (googleClient != null)
                return;

            // Setup connection listener
            var connectionListener = new GoogleConnectionListener();

            // Save main activity for context and stuffs
	        context = activity;

            // Setup sign in options
	        signInOptions = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultGamesSignIn)
	            .RequestScopes(DriveClass.ScopeAppfolder)
	            .Build();

            // Create google client
	        googleClient = new GoogleApiClient.Builder(activity)
	            .AddConnectionCallbacks(connectionListener)
	            .AddApi(Auth.GOOGLE_SIGN_IN_API, signInOptions)
	            .AddApi(GamesClass.API)
	            .Build();

            googleClient.Connect(GoogleApiClient.SignInModeOptional);

            // Wait until we connected and attempt to sign in silently when we do
            connectionListener.Connected += async hint =>
	        {
                var silentSignIn = await Auth.GoogleSignInApi.SilentSignIn(googleClient);
				
				if (silentSignIn.Status.IsSuccess)
	                OnSuccess?.Invoke(silentSignIn.Status);
	            else
	                OnFailure?.Invoke(silentSignIn.Status);

				snapshotManager = new SnapshotManager(googleClient);

				// Try to load from save file, otherwise, create new user
				// TODO
				CurrentUser = new User();
	        };

            // Register callback to our connection listener
            googleClient.RegisterConnectionCallbacks(connectionListener);
        }

        /// <summary>
		/// Get intent used to sign in with Google
		/// </summary>
		public static Intent GetSignInIntent() => 
			Auth.GoogleSignInApi.GetSignInIntent(googleClient);

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

        public static Intent AchievementsIntent => 
            GamesClass.Achievements.GetAchievementsIntent(googleClient);

        public static void UnlockAchievement(string id) => 
            GamesClass.Achievements.Unlock(googleClient, id);

        public static void IncrementAchievement(string id, int steps) =>
            GamesClass.Achievements.Increment(googleClient, id, steps);

        public static async Task<IEnumerable<IAchievement>> GetAchievementsAsync() => 
            (await GamesClass.Achievements.LoadAsync(googleClient, true)).Achievements;

        public static void SetViewForPopups(View view) => 
            GamesClass.SetViewForPopups(googleClient, view);

        public static void SaveUserProgress() => 
            snapshotManager.SaveSnapshotAsync(CurrentUser.ToBytes());
		
		/// <summary>
		/// Tries to load user save async, returns null on failure
		/// </summary>
        public static async Task<User> GetUserProgressOrDefaultAsync() => 
	        User.FromBytes((await snapshotManager.LoadSnapshotAsync()).SnapshotContents.ReadFully());

		public static void ResetUserProgress() => 
            CurrentUser = new User();
    }

    public static class DateTimeConverter
    {
        public static long ToTimestamp(this DateTime dateTime) => 
            (long) (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
    }
}