using Android.App;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Drive;
using Android.Gms.Extensions;
using Android.Gms.Fitness;
using Android.Gms.Games;
using Android.Gms.Tasks;
using Android.Support.V7.App;

namespace CubeQuest.Account
{
    public class AccountManagerBeta
    {
        private static GoogleSignInClient _googleSignIn;

        private static GoogleSignInAccount _googleAccount;

        public static bool IsConnected => _googleAccount != null;

        public static async void Create(AppCompatActivity activity)
        {
            // Ignore if it has already been created
            if (_googleSignIn != null)
                return;

            // Setup connection listener
            var connectionListener = new ConnectionListener();

            // Setup sign in options
            var signInOptions = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultGamesSignIn)
                .RequestScopes(DriveClass.ScopeAppfolder)
                .RequestScopes(FitnessClass.ScopeActivityRead)
                .RequestScopes(GamesClass.ScopeGames)
                .Build();

            _googleSignIn = GoogleSignIn.GetClient(activity, signInOptions);

            var signInListener = new SignInListener();
            signInListener.Complete += account =>
            {
                _googleAccount = account;
            };

            var silent = _googleSignIn.SilentSignIn();
            await silent.AddOnCompleteListener(signInListener);
        }

        public static void RequestGamesPermission(Activity activity)
        {
            if (!GoogleSignIn.HasPermissions(_googleAccount, GamesClass.ScopeGames))
            {
                GoogleSignIn.RequestPermissions(activity, 9005, _googleAccount, GamesClass.ScopeGames);
            }
        }

        public static bool HasGamesPermission =>
            GoogleSignIn.HasPermissions(_googleAccount, GamesClass.ScopeGames);

        public static bool HasGamesLitePermission =>
            GoogleSignIn.HasPermissions(_googleAccount, GamesClass.ScopeGamesLite);
    }

    public class SignInListener : Java.Lang.Object, IOnCompleteListener
    {
        public delegate void CompleteEvent(GoogleSignInAccount account);

        public event CompleteEvent Complete;

        public void OnComplete(Task p0) =>
            Complete?.Invoke(p0.Result as GoogleSignInAccount);
    }
}