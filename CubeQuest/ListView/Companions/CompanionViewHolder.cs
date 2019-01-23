using Android.Support.V7.Widget;
using Android.Widget;

namespace CubeQuest.ListView.Companions
{
    public class CompanionViewHolder : RecyclerView.ViewHolder
    {
        public readonly TextView Name;

        public ImageView Icon { get; }

        public TextView Info { get; }

        public ImageButton ExpandCollapse { get; }

        public LinearLayout SelectablePart { get; }

        public CompanionViewHolder(Android.Views.View companionView) : base(companionView)
        {
            Name = companionView.FindViewById<TextView>(Resource.Id.item_name_text);
            Icon = companionView.FindViewById<ImageView>(Resource.Id.item_icon);
            ExpandCollapse = companionView.FindViewById<ImageButton>(Resource.Id.item_expand_button);
            Info = companionView.FindViewById<TextView>(Resource.Id.item_info);
            SelectablePart = companionView.FindViewById<LinearLayout>(Resource.Id.selector_part);
        }
    }
}