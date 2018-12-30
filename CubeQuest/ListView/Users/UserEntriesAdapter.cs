using System.Collections.Generic;
using System.Linq;
using Android.Content.Res;
using Android.Support.V7.Widget;
using Android.Views;

namespace CubeQuest.ListView.Users
{
	public class UserEntriesAdapter : RecyclerView.Adapter
	{
		public delegate void OnItemClickEvent(UserEntryViewHolder viewHolder);

		public event OnItemClickEvent OnItemClick;

		private readonly List<UserEntry> userEntries;

		private readonly AssetManager assets;

		public UserEntriesAdapter(IEnumerable<UserEntry> entries, AssetManager assetManager)
		{
			userEntries = entries.ToList();
			assets = assetManager;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			// Inflate item layout and create holder
			var inflater = LayoutInflater.From(parent.Context);
			var entry = inflater.Inflate(Resource.Layout.view_user_entry, parent, false);

			var viewHolder = new UserEntryViewHolder(entry);
			viewHolder.Click += view => OnItemClick?.Invoke(viewHolder);

			return viewHolder;
		}

		public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			// Set the view attributes based on data
			var entry = userEntries[position];

			if (!(holder is UserEntryViewHolder userHolder))
				return;

			userHolder.Icon.SetImageBitmap(await entry.GetIconBitmapAsync(assets));
			userHolder.Title.Text = entry.Title;
			userHolder.Description.Text = entry.Description;
		}

		public override int ItemCount => 
			userEntries.Count;
	}
}