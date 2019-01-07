using Android.Support.V7.Widget;
using Android.Widget;

namespace CubeQuest.ListView.Companion
{
    public class CompanionViewHolder : RecyclerView.ViewHolder
    {
        private readonly CardView companionCard;

        public readonly TextView Name;

        public ImageView Icon { get; }

        public TextView Info { get; }

        public ImageButton expandCollapse { get; }

        public CompanionViewHolder(Android.Views.View companionView) : base(companionView)
        {
            companionCard = companionView.FindViewById<CardView>(Resource.Id.card_item_view);
            Name = companionView.FindViewById<TextView>(Resource.Id.item_name_text);
            Icon = companionView.FindViewById<ImageView>(Resource.Id.item_icon);
            expandCollapse = companionView.FindViewById<ImageButton>(Resource.Id.item_expand_button);
            Info = companionView.FindViewById<TextView>(Resource.Id.item_info);
        }
    }
}