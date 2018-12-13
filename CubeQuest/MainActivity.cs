using Android.App;
using Android.Content;
using Android.Gms.Common;
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

            // If successful, launch game
            AccountManager.OnSuccess += status => StartActivity(typeof(GameActivity));

            // Show login button and notice if it failed
            AccountManager.OnFailure += status => ToggleConnecting(false);

            // Google signin
            AccountManager.Create(this);

            // Sign in button
            var signInButton = FindViewById<SignInButton>(Resource.Id.button_sign_in);
            signInButton.SetColorScheme(SignInButton.ColorDark);
            signInButton.SetSize(SignInButton.SizeWide);

            // Start signin intent when clicking on 'sign in'
            signInButton.Click += (sender, args) =>
            {
                StartActivityForResult(AccountManager.GetSignInIntent(), RcSignIn);
            };
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

        private void ToggleConnecting(bool enabled)
        {
            FindViewById<ProgressBar>(Resource.Id.progress_bar_connecting).Visibility = enabled ? ViewStates.Visible : ViewStates.Gone;

            FindViewById<SignInButton>(Resource.Id.button_sign_in).Visibility = enabled ? ViewStates.Gone : ViewStates.Visible;
            FindViewById<TextView>(Resource.Id.text_login_notice).Visibility  = enabled ? ViewStates.Gone : ViewStates.Visible;
        }
    }
}