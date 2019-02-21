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
		/// <summary>
		/// All companions in our inventory
		/// </summary>
        private readonly List<ICompanion> companions;

		/// <summary>
		/// Parent context
		/// </summary>
        private readonly Context context;

		/// <summary>
		/// Buttons for every equipped companion
		/// </summary>
		private RadioButton[] companionButtons;

        public CompanionViewAdapter(List<ICompanion> companions, Context parent)
        {
            this.companions = companions;
            context = parent;
        }

        public override int ItemCount => companions.Count;

        /// <summary>
		/// Use <see cref="SelectedIndex"/>
		/// </summary>
        private int selectedIndex = -1;

        /// <summary>
        /// Currently selected index
        /// </summary>
		private int SelectedIndex
        {
	        get => selectedIndex;

	        set
	        {
				// Set else as not checked
				for (var i = 0; i < companionButtons.Length; i++)
					if (i != value)
						companionButtons[i].Checked = false;

				// Update selected slot var
				selectedIndex = value;
			}
        }

        public event EventHandler EquippedCompanionChanged;

		/// <summary>
		/// Last shown dialog, just so we don't show multiple at once
		/// </summary>
		private AlertDialog dialog;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int viewType)
        {
			// Cast holder
            if (!(holder is CompanionViewHolder viewHolder))
                return;

            var position = viewHolder.AdapterPosition;

			// Set companion name
            viewHolder.Name.Text = companions[position].Name;

			// Set companion type icon
			viewHolder.TypeIcon.SetImageDrawable(AssetLoader.GetCompanionTypeDrawable(context.Resources, companions[position].Type));

            // Set companion icon
            viewHolder.Icon.SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.Companions[position]));

			// Default to down arrow
            viewHolder.ExpandCollapse.SetImageResource(Resource.Drawable.ic_chevron_down);

			// Set companion info
            viewHolder.Info.Text = companions[position].Info;

			// Hide info by default
            viewHolder.Info.Visibility = ViewStates.Gone;

			// We clicked the item
            viewHolder.Click += (args, pos) =>
            {
				// Fix for opening multiple dialogs
				if (dialog?.IsShowing ?? false)
					return;

				// Create dialog
	            var dialogView = CreateDialogView(pos);

				// Show it
				dialog = new AlertDialog.Builder(context, Resource.Style.AlertDialogStyle)
	                .SetTitle("Select Companion to Replace")
                    .SetView(dialogView)
                    .SetPositiveButton("Apply", (o, ee) =>
	                {
						// Change item if valid
						if (SelectedIndex < 0 || SelectedIndex > 2)
							return;

						// Save equipped as temp
                        var temp = AccountManager.CurrentUser.EquippedCompanions[SelectedIndex];

                        // Replace with the one we wanted
                        AccountManager.CurrentUser.EquippedCompanions[SelectedIndex] = companions[pos];

                        // Take equipped one back to inventory
                        companions[pos] = temp;

                        // Notify recycler dialogView we updated an item
                        NotifyItemChanged(pos);

                        // Trigger event
                        EquippedCompanionChanged?.Invoke(this, null);
                    })
                    .SetNegativeButton("Cancel", (o, ee) => SelectedIndex = -1)
                    .Show();
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
			// Inflate dialogView holder
			return new CompanionViewHolder(LayoutInflater.From(parent.Context)
		        .Inflate(Resource.Layout.view_item_card, parent, false));
        }

		private View CreateDialogView(int position)
        {
			// Set dialogView
			var inflater = LayoutInflater.From(context);
			var dialogView = inflater.Inflate(Resource.Layout.view_dialog_companion, null);

			// Inflate stubs
			var companionStubs = new[]
			{
				dialogView.FindViewById<ViewStub>(Resource.Id.stub_companion_dialog_1).Inflate(),
				dialogView.FindViewById<ViewStub>(Resource.Id.stub_companion_dialog_2).Inflate(),
				dialogView.FindViewById<ViewStub>(Resource.Id.stub_companion_dialog_3).Inflate()
			};

			// Companion radio buttons
			companionButtons = new[]
			{
				companionStubs[0].FindViewById<RadioButton>(Resource.Id.radio_companion_dialog),
				companionStubs[1].FindViewById<RadioButton>(Resource.Id.radio_companion_dialog),
				companionStubs[2].FindViewById<RadioButton>(Resource.Id.radio_companion_dialog)
			};

			// Set click events
			companionButtons[0].Click += (sender, args) => SelectedIndex = 0;
			companionButtons[1].Click += (sender, args) => SelectedIndex = 1;
			companionButtons[2].Click += (sender, args) => SelectedIndex = 2;

			// Set companion image views
			var companionImages = new[]
			{
				companionStubs[0].FindViewById<ImageView>(Resource.Id.image_companion_dialog),
				companionStubs[1].FindViewById<ImageView>(Resource.Id.image_companion_dialog),
				companionStubs[2].FindViewById<ImageView>(Resource.Id.image_companion_dialog)
			};

			var companionNames = new[]
			{
				companionStubs[0].FindViewById<TextView>(Resource.Id.text_companion_dialog_name),
				companionStubs[1].FindViewById<TextView>(Resource.Id.text_companion_dialog_name),
				companionStubs[2].FindViewById<TextView>(Resource.Id.text_companion_dialog_name)
			};

			// Just a shorter name
			var equippedCompanions = AccountManager.CurrentUser.EquippedCompanions;

			// Set companion images
			for (var i = 0; i < companionImages.Length; i++)
			{
				// Set image
				companionImages[i].SetImageBitmap(AssetLoader.GetCompanionBitmap(equippedCompanions[i]));

				// Set name
				companionNames[i].Text = equippedCompanions[i].Name;

				// Set stats
				var attack  = companions[position].Attack  - equippedCompanions[i].Attack;
				var health  = companions[position].Health  - equippedCompanions[i].Health;
				var armor   = companions[position].Armor   - equippedCompanions[i].Armor;
				var evasion = companions[position].Evasion - equippedCompanions[i].Evasion;

				companionStubs[i].FindViewById<TextView>(Resource.Id.text_companion_attack).Text  = $"{(attack  > 0 ? "+" : "")}{attack}";
				companionStubs[i].FindViewById<TextView>(Resource.Id.text_companion_health).Text  = $"{(health  > 0 ? "+" : "")}{health}";
				companionStubs[i].FindViewById<TextView>(Resource.Id.text_companion_armor).Text   = $"{(armor   > 0 ? "+" : "")}{armor}";
				companionStubs[i].FindViewById<TextView>(Resource.Id.text_companion_evasion).Text = $"{(evasion > 0 ? "+" : "")}{evasion * 100}%";
			}

			return dialogView;
        }
    }
}