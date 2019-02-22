using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using CubeQuest.Account.Interface;

namespace CubeQuest.Handler
{
	public static class Alert
	{
		public static AlertDialog.Builder Build(Context context) => 
			new AlertDialog.Builder(context, Resource.Style.AlertDialogStyle);

		/// <summary>
		/// Builds a simple alert dialog and sets the title and positive button
		/// </summary>
		private static AlertDialog.Builder BuildSimple(Context context, string title) =>
			Build(context)
				.SetTitle(title)
				.SetPositiveButton("OK", (IDialogInterfaceOnClickListener) null);

		private static AlertDialog.Builder BuildSimple(Context context, int titleResource) =>
			Build(context)
				.SetTitle(titleResource)
				.SetPositiveButton("OK", (IDialogInterfaceOnClickListener)null);

		/// <summary>
		/// Builds a simple alert dialog and shows it to the user
		/// </summary>
		public static void ShowSimple(Context context, string title, string message) =>
			BuildSimple(context, title)
				.SetMessage(message)
				.Show();
		
		public static void ShowSimple(Context context, string title, View view) =>
			BuildSimple(context, title)
				.SetView(view)
				.Show();

		public static void ShowSimple(Context context, int titleResource, View view)
		{
			BuildSimple(context, titleResource)
				.SetView(view)
				.Show();
		}

		public static void ShowCompanionInfo(Activity context, ICompanion companion)
		{
			var view = context.LayoutInflater.Inflate(Resource.Layout.view_dialog_companion_info, null);

			// Set companion icon
			view.FindViewById<ImageView>(Resource.Id.image_companion_dialog_icon)
				.SetImageBitmap(AssetLoader.GetCompanionBitmap(companion));

			// Set companion type icon
			view.FindViewById<ImageView>(Resource.Id.image_companion_dialog_type)
				.SetImageDrawable(AssetLoader.GetCompanionTypeDrawable(context.Resources, companion.Type));

			// Set companion info
			view.FindViewById<TextView>(Resource.Id.text_companion_dialog_info).Text = companion.Info;

			// Set stats
			view.FindViewById<TextView>(Resource.Id.text_companion_dialog_health).Text  = $"{companion.Health}";
			view.FindViewById<TextView>(Resource.Id.text_companion_dialog_armor).Text   = $"{companion.Armor}";
			view.FindViewById<TextView>(Resource.Id.text_companion_dialog_attack).Text  = $"{companion.Attack}";
			view.FindViewById<TextView>(Resource.Id.text_companion_dialog_evasion).Text = $"{companion.Evasion * 100}%";

			// Show it
			ShowSimple(context, companion.Name, view);
		}
	}
}