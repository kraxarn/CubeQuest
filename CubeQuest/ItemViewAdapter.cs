using System;
using System.Collections.Generic;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CubeQuest.Account.Interface;

namespace CubeQuest
{
    public class ItemViewAdapter : RecyclerView.Adapter
    {

        List<IItem> items;

        public ItemViewAdapter(List<IItem> items)
        {
            this.items = items;
        }

        public override int ItemCount => items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ItemViewHolder itemHolder = holder as ItemViewHolder;
            itemHolder.name.Text = items[position].Name;
            //Replace below line with something that gets the real icon
            itemHolder.icon.SetImageResource(Android.Resource.Drawable.ArrowDownFloat);
            itemHolder.info.Text = items[position].Info;

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View vv = LayoutInflater.
                From(parent.Context).
                Inflate(Resource.Layout.view_item_card, parent, false);

            return new ItemViewHolder(vv);
        }
    }
}
