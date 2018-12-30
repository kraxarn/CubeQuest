using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace CubeQuest.ListView.Users
{
	public class UserEntryViewHolder : RecyclerView.ViewHolder
	{
		public delegate void ClickEvent(View itemView);

		public event ClickEvent Click;

		public ImageView Icon { get; }

		public TextView Title { get; }

		public TextView Description { get; }

		public UserEntryViewHolder(View itemView) : base(itemView)
		{
			Icon  = itemView.FindViewById<ImageView>(Resource.Id.image_user_entry_icon);
			Title = itemView.FindViewById<TextView>(Resource.Id.text_user_entry_title);
			Description = itemView.FindViewById<TextView>(Resource.Id.text_user_entry_description);

			itemView.Click += (sender, args) => 
				Click?.Invoke(itemView);
		}
	}
}