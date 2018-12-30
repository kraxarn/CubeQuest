using Android.Support.V7.Widget;
using Android.Widget;

namespace CubeQuest.ListView.Item
{
	public class ItemViewHolder : RecyclerView.ViewHolder
    {
        private readonly CardView itemCard;

        public readonly TextView Name;

        public ImageView Icon { get; }

        public TextView Info { get; }

        public ItemViewHolder(Android.Views.View itemView) : base(itemView)
        {
            itemCard = itemView.FindViewById<CardView>(Resource.Id.card_item_view);
            Name     = itemView.FindViewById<TextView>(Resource.Id.item_name_text);
            Icon     = itemView.FindViewById<ImageView>(Resource.Id.item_icon);
            Info     = itemView.FindViewById<TextView>(Resource.Id.item_info);
        }
    }
}