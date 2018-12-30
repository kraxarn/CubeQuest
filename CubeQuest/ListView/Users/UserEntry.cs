using Android.Content.Res;
using Android.Graphics;
using System.Threading.Tasks;

namespace CubeQuest.ListView.Users
{
	public class UserEntry
	{
		/// <summary>
		/// Path to the image
		/// </summary>
		public string Icon { get; }

		public async Task<Bitmap> GetIconBitmapAsync(AssetManager assets)
		{
			if (Icon == null)
				return null;

			return await BitmapFactory.DecodeStreamAsync(assets.Open(Icon));
		}

		public string Title { get; }

		public string Description { get; }

		public UserEntry(string icon, string title, string description)
		{
			Icon        = icon;
			Title       = title;
			Description = description;
		}
	}
}