using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using CubeQuest.Handler;
using CubeQuest.Account;
using CubeQuest.Account.Interface;

namespace CubeQuest.ListView.Companions
{
    public class CompanionViewAdapter : RecyclerView.Adapter
    {
        private readonly List<ICompanion> companions;

        public CompanionViewAdapter(List<ICompanion> companions) =>
            this.companions = companions;

        public override int ItemCount => companions.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (!(holder is CompanionViewHolder companionHolder))
                return;

            companionHolder.Name.Text = companions[position].Name;

            //Replace below line with something that gets the real icon
            companionHolder.Icon.SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.Companions[position]));

            companionHolder.ExpandCollapse.SetImageResource(Resource.Drawable.ic_numbered_list);
            companionHolder.Info.Text = companions[position].Info;
            companionHolder.Info.Visibility = ViewStates.Gone;

            //Makes info text disappear or appear on the click
            companionHolder.ExpandCollapse.Click += (sender, args) => {
                if (companionHolder.Info.Visibility == ViewStates.Gone)
                {
                    companionHolder.Info.Visibility = ViewStates.Visible;
                    companionHolder.ExpandCollapse.SetImageResource(Resource.Drawable.ic_shield);
                }
                else
                {
                    companionHolder.Info.Visibility = ViewStates.Gone;
                    companionHolder.ExpandCollapse.SetImageResource(Resource.Drawable.ic_numbered_list);
                }
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
            new CompanionViewHolder(LayoutInflater
                .From(parent.Context)
                .Inflate(Resource.Layout.view_item_card, parent, false));
    }
}
