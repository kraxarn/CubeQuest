using Android.Support.V7.Widget;
using Android.Views;
using CubeQuest.Account.Interface;
using System.Collections.Generic;

namespace CubeQuest
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
            itemHolder.Icon.SetImageResource(Android.Resource.Drawable.ArrowDownFloat);
            itemHolder.Info.Text = items[position].Info;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
	        new ItemViewHolder(LayoutInflater
		        .From(parent.Context)
		        .Inflate(Resource.Layout.view_item_card, parent, false));
    }
}
