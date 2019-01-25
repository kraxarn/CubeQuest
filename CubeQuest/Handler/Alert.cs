using Android.Content;
using Android.Views;
using Android.Support.V7.App;

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
	}
}