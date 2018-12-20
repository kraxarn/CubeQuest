using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace CubeQuest
{
    public class ItemViewHolder : RecyclerView.ViewHolder
    {
        CardView itemCard;

        public TextView name;
        public ImageView icon { get; set; }

        public TextView info { get; set; }

        public ItemViewHolder(View itemView) : base(itemView)
        {
            itemCard = (CardView)itemView.FindViewById(Resource.Id.card_item_view);
            name = (TextView)itemView.FindViewById(Resource.Id.item_name_text);
            icon = (ImageView)itemView.FindViewById(Resource.Id.item_icon);

            info = (TextView)itemView.FindViewById(Resource.Id.item_info);


        }
    }
}
