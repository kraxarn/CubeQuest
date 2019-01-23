using Android.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CubeQuest.Account;
using CubeQuest.Account.Interface;
using CubeQuest.Handler;
using System;
using System.Collections.Generic;

namespace CubeQuest.ListView.Companions
{
	public class CompanionViewAdapter : RecyclerView.Adapter
    {
        private readonly List<ICompanion> companions;

        private readonly Context parentGroup;

        private ImageView slot1Occupant;
        private ImageView slot2Occupant;
        private ImageView slot3Occupant;

        public CompanionViewAdapter(List<ICompanion> companions, Context parent)
        {
            this.companions = companions;
            parentGroup = parent;
            CreateInsertView();
        }

        public override int ItemCount => companions.Count;

        // selectedSlot is initialized to 8 because it is an unusable value
        private int selectedSlot = 8;

        private View companionInsertView;

        private CompanionViewHolder companionHolder;

        private AlertDialog itemPopupDialog;

        public event EventHandler EquippedCompanionChanged;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (!(holder is CompanionViewHolder viewHolder))
                return;

            viewHolder.Name.Text = companions[position].Name;

            //Replace below line with something that gets the real icon
            viewHolder.Icon.SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.Companions[position]));

            viewHolder.ExpandCollapse.SetImageResource(Resource.Drawable.ic_numbered_list);
            viewHolder.Info.Text = companions[position].Info;
            viewHolder.Info.Visibility = ViewStates.Gone;

            //Makes info text disappear or appear on the click
            viewHolder.ExpandCollapse.Click += (sender, args) =>
            {
                if (viewHolder.Info.Visibility == ViewStates.Gone)
                {
                    viewHolder.Info.Visibility = ViewStates.Visible;
                    viewHolder.ExpandCollapse.SetImageResource(Resource.Drawable.ic_shield);
                }
                else
                {
                    viewHolder.Info.Visibility = ViewStates.Gone;
                    viewHolder.ExpandCollapse.SetImageResource(Resource.Drawable.ic_numbered_list);
                }
            };
            viewHolder.SelectablePart.Click += (sender, e) =>
            {
                if (itemPopupDialog == null)
                {
                    itemPopupDialog = new AlertDialog.Builder(parentGroup)
                        .SetView(companionInsertView)
                        .SetPositiveButton("Apply", (o, ee) =>
                        {
                            /*
                             Insert code that makes the users choice of item from the
                             list become their selected equipment
                            */
                            if (selectedSlot >= 0 && selectedSlot < 3)
                            {
                                ICompanion tempEquip = AccountManager.CurrentUser.EquippedCompanions[selectedSlot];
                                AccountManager.CurrentUser.EquippedCompanions[selectedSlot] = GetCompanion(position);
                                companions[position] = tempEquip;

                                slot1Occupant.SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[0]));
                                slot2Occupant.SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[1]));
                                slot3Occupant.SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[2]));
                                NotifyItemChanged(position);

                                EquippedCompanionChanged?.Invoke(this, null);
                            }
                        })
                        .SetNegativeButton("Cancel", (o, ee) =>
                        {
                            //Insert code for closing dialog without any updates to chosen equipment
                            selectedSlot = 5;
                        })
                        .Create();
                }

                itemPopupDialog.Show();
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
            companionHolder = new CompanionViewHolder(LayoutInflater
                .From(parent.Context)
                .Inflate(Resource.Layout.view_item_card, parent, false));

            return companionHolder;
        }

        public ICompanion GetCompanion(int position) => companions[position];

        private void CreateInsertView()
        {
            // Set up companionInsertView 
            var inflater = LayoutInflater.From(parentGroup);
            companionInsertView = inflater.Inflate(Resource.Layout.view_insert_companion, null);

            var itemSlot1 = companionInsertView.FindViewById<LinearLayout>(Resource.Id.companion_slot_1);
            var itemSlot2 = companionInsertView.FindViewById<LinearLayout>(Resource.Id.companion_slot_2);
            var itemSlot3 = companionInsertView.FindViewById<LinearLayout>(Resource.Id.companion_slot_3);

            itemSlot1.Click += (sender, e) =>
            {
                itemSlot2.SetBackgroundColor(Android.Graphics.Color.Transparent);
                itemSlot3.SetBackgroundColor(Android.Graphics.Color.Transparent);
                itemSlot1.SetBackgroundResource(Resource.Drawable.backgrnd_rounded_corners);
                selectedSlot = 0;
            };
            itemSlot2.Click += (sender, e) =>
            {
                itemSlot2.SetBackgroundResource(Resource.Drawable.backgrnd_rounded_corners);
                itemSlot1.SetBackgroundColor(Android.Graphics.Color.Transparent);
                itemSlot3.SetBackgroundColor(Android.Graphics.Color.Transparent);
                selectedSlot = 1;
            };
            itemSlot3.Click += (sender, e) =>
            {
                itemSlot3.SetBackgroundResource(Resource.Drawable.backgrnd_rounded_corners);
                itemSlot2.SetBackgroundColor(Android.Graphics.Color.Transparent);
                itemSlot1.SetBackgroundColor(Android.Graphics.Color.Transparent);
                selectedSlot = 2;
            };

            slot1Occupant = companionInsertView.FindViewById<ImageView>(Resource.Id.slot_1_occupant);
            slot2Occupant = companionInsertView.FindViewById<ImageView>(Resource.Id.slot_2_occupant);
            slot3Occupant = companionInsertView.FindViewById<ImageView>(Resource.Id.slot_3_occupant);

            slot1Occupant.SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[0]));
            slot2Occupant.SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[1]));
            slot3Occupant.SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[2]));
        }
    }
}