using Android.Support.V7.Widget;
using Android.Widget;

namespace CubeQuest.ListView.Item
{
	public class ItemViewHolder : RecyclerView.ViewHolder
    {
        public readonly TextView Name;

        public ImageView Icon { get; }

        public TextView Info { get; }

        public ImageButton ExpandCollapse { get; }

        public ItemViewHolder(Android.Views.View itemView) : base(itemView)
        {
            Name           = itemView.FindViewById<TextView>(Resource.Id.item_name_text);
            Icon           = itemView.FindViewById<ImageView>(Resource.Id.item_icon);
            ExpandCollapse = itemView.FindViewById<ImageButton>(Resource.Id.item_expand_button);
            Info           = itemView.FindViewById<TextView>(Resource.Id.item_info);
        }
    }
}