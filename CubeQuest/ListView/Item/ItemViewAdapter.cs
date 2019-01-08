using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using CubeQuest.Account.Interface;

namespace CubeQuest.ListView.Item
{
	public class ItemViewAdapter : RecyclerView.Adapter
    {
	    private readonly List<IItem> items;

        public ItemViewAdapter(List<IItem> items) => 
	        this.items = items;

        public override int ItemCount => items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (!(holder is ItemViewHolder itemHolder))
                return;

            itemHolder.Name.Text = items[position].Name;

            //Replace below line with something that gets the real icon
            itemHolder.Icon.SetImageResource(Resource.Drawable.ic_companion);

            itemHolder.ExpandCollapse.SetImageResource(Resource.Drawable.ic_numbered_list);
            itemHolder.Info.Text = items[position].Info;
            itemHolder.Info.Visibility = ViewStates.Gone;

            //Makes info text disappear or appear on the click
            itemHolder.ExpandCollapse.Click += (sender, args) => {
            if(itemHolder.Info.Visibility == ViewStates.Gone)
                {
                    itemHolder.Info.Visibility = ViewStates.Visible;
                    itemHolder.ExpandCollapse.SetImageResource(Resource.Drawable.ic_shield);
                }
                else
                {
                    itemHolder.Info.Visibility = ViewStates.Gone;
                    itemHolder.ExpandCollapse.SetImageResource(Resource.Drawable.ic_numbered_list);
                }
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
	        new ItemViewHolder(LayoutInflater
		        .From(parent.Context)
		        .Inflate(Resource.Layout.view_item_card, parent, false));
    }
}
