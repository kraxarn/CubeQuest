﻿using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Support.V7.App;

namespace CubeQuest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private AccountManager signInManager;

        private const int RcSignIn = 9001;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Google signin
            signInManager = new AccountManager(this);

			// Sign in button
	        var signInButton = FindViewById<SignInButton>(Resource.Id.button_sign_in);
			signInButton.SetColorScheme(SignInButton.ColorDark);
			signInButton.SetSize(SignInButton.SizeWide);

            signInButton.Click += (sender, args) =>
            {
                var intent = signInManager.GetSignInIntent();
                StartActivityForResult(intent, RcSignIn);
            };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == RcSignIn)
                signInManager.HandleResult(data);
        }
    }
}

