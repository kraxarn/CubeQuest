using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;

namespace CubeQuest.Account
{
    public class GoogleConnectionListener : Java.Lang.Object, GoogleApiClient.IOnConnectionFailedListener, GoogleApiClient.IConnectionCallbacks
    {
        public delegate void ConnectionFailedEvent(ConnectionResult result);

        public event ConnectionFailedEvent Failed;

        public delegate void ConnectedEvent(Bundle connectionHint);

        public event ConnectedEvent Connected;

        public delegate void ConnectionSuspendedEvent(int cause);

        public event ConnectionSuspendedEvent Suspended;

        public void OnConnectionFailed(ConnectionResult result) =>
            Failed?.Invoke(result);

        public void OnConnected(Bundle connectionHint) =>
            Connected?.Invoke(connectionHint);

        public void OnConnectionSuspended(int cause) => 
            Suspended?.Invoke(cause);
    }
}