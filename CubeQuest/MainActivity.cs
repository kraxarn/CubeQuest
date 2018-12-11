using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Support.V7.App;
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

            // Google signin
            AccountManager.Create(this);

            // If successful, launch game
            AccountManager.OnSuccess += status => StartActivity(typeof(GameActivity));

			// Sign in button
	        var signInButton = FindViewById<SignInButton>(Resource.Id.button_sign_in);
			signInButton.SetColorScheme(SignInButton.ColorDark);
			signInButton.SetSize(SignInButton.SizeWide);

            // Start signin intent when clicking on 'sign in'
            // TODO: Try to sign in silently
            signInButton.Click += (sender, args) => StartActivityForResult(AccountManager.GetSignInIntent(), RcSignIn);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Send result to AccountManager
            if (requestCode == RcSignIn)
                AccountManager.HandleResult(data);
        }
    }
}