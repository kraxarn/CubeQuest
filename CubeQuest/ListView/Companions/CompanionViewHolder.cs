using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace CubeQuest.ListView.Companions
{
	public class CompanionViewHolder : RecyclerView.ViewHolder
    {
	    public delegate void ClickEvent(View itemView);

	    public event ClickEvent Click;

		public TextView Name { get; }

        public ImageView Icon { get; }

        public TextView Info { get; }

        public ImageButton ExpandCollapse { get; }

        public LinearLayout SelectablePart { get; }

        public CompanionViewHolder(View companionView) : base(companionView)
        {
            Name           = companionView.FindViewById<TextView>(Resource.Id.item_name_text);
            Icon           = companionView.FindViewById<ImageView>(Resource.Id.item_icon);
            ExpandCollapse = companionView.FindViewById<ImageButton>(Resource.Id.item_expand_button);
            Info           = companionView.FindViewById<TextView>(Resource.Id.item_info);
            SelectablePart = companionView.FindViewById<LinearLayout>(Resource.Id.selector_part);

			// Show or hide text on click
			ExpandCollapse.Click += (sender, args) =>
            {
	            if (Info.Visibility == ViewStates.Gone)
	            {
		            Info.Visibility = ViewStates.Visible;
		            ExpandCollapse.SetImageResource(Resource.Drawable.ic_chevron_up);
	            }
	            else
	            {
		            Info.Visibility = ViewStates.Gone;
		            ExpandCollapse.SetImageResource(Resource.Drawable.ic_chevron_down);
	            }
            };

			// Clicking on item
            SelectablePart.Click += (sender, args) => 
	            Click?.Invoke(companionView);
		}
	}
}