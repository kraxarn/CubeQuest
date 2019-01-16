using Android.Content;
using Android.Views;

namespace CubeQuest.Handler
{
	public static class AlertDialog
	{
		public static Android.Support.V7.App.AlertDialog.Builder Build(Context context) => 
			new Android.Support.V7.App.AlertDialog.Builder(context, Resource.Style.AlertDialogStyle);

		/// <summary>
		/// Builds a simple alert dialog and sets the title and positive button
		/// </summary>
		private static Android.Support.V7.App.AlertDialog.Builder BuildSimple(Context context, string title) =>
			Build(context)
				.SetTitle(title)
				.SetPositiveButton("OK", (IDialogInterfaceOnClickListener) null);

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
	}
}