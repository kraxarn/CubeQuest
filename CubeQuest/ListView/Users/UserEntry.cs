using Android.Graphics;
using CubeQuest.Handler;

namespace CubeQuest.ListView.Users
{
    public class UserEntry
	{
		/// <summary>
		/// Path to the image
		/// </summary>
		public string Icon { get; }

        public Bitmap IconBitmap => 
            Icon == null ? null : AssetLoader.GetBitmap(Icon);

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