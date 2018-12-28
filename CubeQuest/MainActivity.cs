using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Net;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using CubeQuest.Account;
using System.Collections.Generic;
using Uri = Android.Net.Uri;

namespace CubeQuest
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int RcSignIn = 9001;
		
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
			
			// Check if Google Play Games is installed
			if (!IsPackageInstalled("com.google.android.play.games"))
			{
				// Toggle error message and connecting spinner
				ToggleVisibilities(new List<int>
				{
					Resource.Id.text_play_games_error,
					Resource.Id.progress_bar_connecting
				});

				// Get button to install it
				var playGamesButton = FindViewById<Button>(Resource.Id.button_install_play_games);

				// Open the item in Google Play when clicking on it
				playGamesButton.Click += (sender, args) => 
					OpenPlayStorePackage("com.google.android.play.games");

				// Show it
				ToggleVisibility(playGamesButton);

				// Don't execute the rest of the code
				return;
			}

            // Sign in button
            var signInButton = FindViewById<SignInButton>(Resource.Id.button_sign_in);
            signInButton.SetColorScheme(SignInButton.ColorDark);
            signInButton.SetSize(SignInButton.SizeWide);

            // If successful, launch game
            AccountManager.OnSuccess += status =>
            {
				if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
					RequestPermissions(new[]
					{
						Manifest.Permission.AccessFineLocation
					}, 0);
				else
					OpenGameActivity();
            };

            // Show login button and notice if it failed
            AccountManager.OnFailure += status => ToggleConnecting(false);

            if (!IsConnected)
            {
                // Elements
                var textInternetError     = FindViewById<TextView>(Resource.Id.text_internet_error);
                var progressBarConnecting = FindViewById<ProgressBar>(Resource.Id.progress_bar_connecting);
                var buttonRetryInternet   = FindViewById<Button>(Resource.Id.button_retry_internet);

                void ShowConnectionError(bool show)
                {
                    textInternetError.Visibility     = show ? ViewStates.Visible : ViewStates.Gone;
                    buttonRetryInternet.Visibility   = show ? ViewStates.Visible : ViewStates.Gone;
                    progressBarConnecting.Visibility = show ? ViewStates.Gone    : ViewStates.Visible;
                }

                // Show/hide elements
                ShowConnectionError(true);

                // Try again if button was pressed
                buttonRetryInternet.Click += (sender, args) =>
                {
                    // Hide error and show progress bar again
                    ShowConnectionError(false);

                    // If connected, try to sign in again
                    if (IsConnected)
                        AccountManager.Create(this);
                    else
                        ShowConnectionError(true);
                };

                // Don't continue with the code
                return;
            }

            // Start signin intent when clicking on 'sign in'
            signInButton.Click += (sender, args) =>
            {
                StartActivityForResult(AccountManager.GetSignInIntent(), RcSignIn);
            };
        }

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			if (grantResults[0] == Permission.Granted)
				OpenGameActivity();
			else
			{
				// TODO: Show some type of error
				Toast.MakeText(this, "Permission request denied", ToastLength.Long);
			}
		}

		private void OpenGameActivity()
        {
	        StartActivity(new Intent(this, typeof(GameActivity)),
		        ActivityOptions.MakeCustomAnimation(this, Android.Resource.Animation.FadeIn,
			        Android.Resource.Animation.FadeOut).ToBundle());
		}

        protected override void OnStart()
        {
            base.OnStart();

            MusicManager.Create(this);
            MusicManager.Volume = 0.4f;
            MusicManager.Play(MusicManager.EMusicTrack.Map);

            // Google signin
            if (IsConnected)
                AccountManager.Create(this);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Send result to AccountManager
            if (requestCode == RcSignIn)
            {
                // Show progress bar again
                ToggleConnecting(true);

                AccountManager.HandleResult(data);
            }
        }

        /// <summary>
        /// If the device is connected to the network
        /// </summary>
        private bool IsConnected => 
            (GetSystemService(ConnectivityService) as ConnectivityManager)?.ActiveNetworkInfo?.IsConnected ?? false;

        private void ToggleConnecting(bool enabled)
        {
            FindViewById<ProgressBar>(Resource.Id.progress_bar_connecting).Visibility = enabled ? ViewStates.Visible : ViewStates.Gone;

            FindViewById<SignInButton>(Resource.Id.button_sign_in).Visibility = enabled ? ViewStates.Gone : ViewStates.Visible;
            FindViewById<TextView>(Resource.Id.text_login_notice).Visibility  = enabled ? ViewStates.Gone : ViewStates.Visible;
        }

		/// <summary>
		/// If the app is running in debug configuration
		/// </summary>
        public static bool DebugMode
        {
	        get
	        {
				#if DEBUG
					return true;
				#else
					return false;
				#endif
	        }
		}

		/// <summary>
		/// Toggles between <see cref="ViewStates.Visible"/> and <see cref="ViewStates.Gone"/>
		/// </summary>
		private static void ToggleVisibility(View view) => 
			view.Visibility = view.Visibility == ViewStates.Visible ? ViewStates.Gone : ViewStates.Visible;
		
		/// <summary>
		/// Toggles visibility by specified ID
		/// </summary>
		private void ToggleVisibility(int viewId) => 
			ToggleVisibility(FindViewById(viewId));

		/// <summary>
		/// Toggle visibility on multiple items
		/// </summary>
		private void ToggleVisibilities(List<int> viewIds) => 
			viewIds.ForEach(ToggleVisibility);

		/// <summary>
		/// Check if specific package name is installed on the device
		/// </summary>
		private bool IsPackageInstalled(string packageName)
		{
			try
			{
				// TODO: We could check version here as well
				PackageManager.GetPackageInfo(packageName, PackageInfoFlags.MetaData);
				return true;
			}
			catch (PackageManager.NameNotFoundException)
			{
				return false;
			}
		}

		/// <summary>
		/// Opens the specified package in the Google Play store
		/// </summary>
		private void OpenPlayStorePackage(string packageName) =>
			StartActivity(new Intent(Intent.ActionView)
				.SetData(Uri.Parse($"https://play.google.com/store/apps/details?id={packageName}"))
				.SetPackage("com.android.vending"));
    }
}