using Android.App;
using Android.OS;
using Android.Widget;

namespace CubeQuest.Layout
{
	[Activity(Label = "CanvasTestActivity", Theme = "@style/AppTheme.NoActionBar")]
	public class CanvasTestActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_canvas_test);

			var canvas = new BattleDrawable(Assets);
			var canvasView = FindViewById<ImageView>(Resource.Id.drawable_canvas_test);
			canvasView.SetImageDrawable(canvas);
		}
	}
}