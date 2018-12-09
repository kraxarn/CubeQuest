using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Support.V7.App;

namespace CubeQuest
{
    [Activity(Label = "GameActivity")]
    public class GameActivity : AppCompatActivity, IOnMapReadyCallback
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_game);

            // Get map and listen when it's ready
            var mapFragment = (SupportMapFragment) SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            var defaultPosition = new LatLng(59.618706, 16.540438);
            googleMap.MoveCamera(CameraUpdateFactory.NewLatLng(defaultPosition));
        }
    }
}