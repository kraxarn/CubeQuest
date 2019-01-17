using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using CubeQuest.Handler;
using CubeQuest.Account;
using CubeQuest.Account.Interface;
using CubeQuest.Account.Companions;
using Android.Widget;
using System;
using Android.Support.V7.App;
using Android.Content;

namespace CubeQuest.ListView.Companions
{
    public class CompanionViewAdapter : RecyclerView.Adapter
    {
        private List<ICompanion> companions;
        private Context parentGroup;

        ImageView slot1Occupant;
        ImageView slot2Occupant;
        ImageView slot3Occupant;

        public CompanionViewAdapter(List<ICompanion> companions, Context parent)
        {
            this.companions = companions;
            companions.Add(new Bear());
            companions.Add(new Parrot());
            parentGroup = parent;
            createInsertView();

        }

        public override int ItemCount => companions.Count;
        //selecteedSlot is initialized to 8 because it is an unusable value
        private int selectedSlot = 8;

        private View companionInsertView;

        private CompanionViewHolder companionHolder;

        private AlertDialog itemPopupDialog;

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
            companionHolder.selectablePart.Click += (sender, e) => {

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
                                AccountManager.CurrentUser.EquippedCompanions[selectedSlot] = getCompanion(position);
                                companions[position] = tempEquip;

                                slot1Occupant.SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[0]));
                                slot2Occupant.SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[1]));
                                slot3Occupant.SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[2]));
                                NotifyItemChanged(position);

                                EquippedCompanionChanged.Invoke(this, null);
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

        public ICompanion getCompanion(int position)
        {
            return companions[position];
        }

        private void createInsertView() {
            //Set up companionInsertView 

            LayoutInflater inflater = LayoutInflater.From(parentGroup);
            companionInsertView = inflater.Inflate(Resource.Layout.view_insert_companion, null);

            var itemSlot1 = companionInsertView.FindViewById<LinearLayout>(Resource.Id.companion_slot_1);
            var itemSlot2 = companionInsertView.FindViewById<LinearLayout>(Resource.Id.companion_slot_2);
            var itemSlot3 = companionInsertView.FindViewById<LinearLayout>(Resource.Id.companion_slot_3);

            itemSlot1.Click += (object sender, EventArgs e) => {
                itemSlot2.SetBackgroundColor(Android.Graphics.Color.Transparent);
                itemSlot3.SetBackgroundColor(Android.Graphics.Color.Transparent);
                itemSlot1.SetBackgroundResource(Resource.Drawable.backgrnd_rounded_corners);
                selectedSlot = 0;
            };
            itemSlot2.Click += (object sender, EventArgs e) => {
                itemSlot2.SetBackgroundResource(Resource.Drawable.backgrnd_rounded_corners);
                itemSlot1.SetBackgroundColor(Android.Graphics.Color.Transparent);
                itemSlot3.SetBackgroundColor(Android.Graphics.Color.Transparent);
                selectedSlot = 1;
            };
            itemSlot3.Click += (object sender, EventArgs e) => {
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

        public event EventHandler EquippedCompanionChanged;

    }
}
