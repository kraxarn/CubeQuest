﻿using Android.Support.V7.Widget;
using Android.Views;
using System.Collections.Generic;
using System.Linq;

namespace CubeQuest.ListView.Users
{
    public class UserEntriesAdapter : RecyclerView.Adapter
	{
		public delegate void ItemClickEvent(UserEntryViewHolder viewHolder);

		public event ItemClickEvent ItemClick;

		private readonly List<UserEntry> userEntries;

		public UserEntriesAdapter(IEnumerable<UserEntry> entries) => 
            userEntries = entries.ToList();

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			// Inflate item layout and create holder
			var inflater = LayoutInflater.From(parent.Context);
			var entry = inflater.Inflate(Resource.Layout.view_user_entry, parent, false);

			var viewHolder = new UserEntryViewHolder(entry);
			viewHolder.Click += view => ItemClick?.Invoke(viewHolder);

			return viewHolder;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			// Set the view attributes based on data
			var entry = userEntries[position];

			if (!(holder is UserEntryViewHolder userHolder))
				return;

			userHolder.Icon.SetImageBitmap(entry.IconBitmap);
			userHolder.Title.Text = entry.Title;
			userHolder.Description.Text = entry.Description;
		}

		public override int ItemCount => 
			userEntries.Count;
	}
}