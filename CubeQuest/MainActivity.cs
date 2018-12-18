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
    }
}