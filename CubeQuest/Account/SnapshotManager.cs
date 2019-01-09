using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Gms.Games.Snapshot;
using Android.Runtime;
using System;
using System.Threading.Tasks;

namespace CubeQuest.Account
{
	public class SnapshotManager
    {
        private readonly GoogleApiClient googleClient;

        private const string CurrentSaveName = "CubeQuestSave";

        public Intent SelectSnapshotIntent => 
	        GamesClass.Snapshots.GetSelectSnapshotIntent(googleClient, "Select Save", false, true, 10);

        public SnapshotManager(GoogleApiClient client) => 
	        googleClient = client;

        private async Task<ISnapshot> GetSnapshot()
        {
            const int policy = Snapshots.ResolutionPolicyMostRecentlyModified;

            // TODO: No idea if this actually works (probably not)
            var x = await GamesClass.Snapshots.Open(googleClient, CurrentSaveName, true, policy);
			
            return x.JavaCast<ISnapshot>();
        }

        private async Task<ISnapshotContents> LoadFromSnapshot() =>
            (await GetSnapshot()).SnapshotContents;

        private async Task<ISnapshotsOpenSnapshotResult> OpenSnapshot() => 
	        await GamesClass.Snapshots.OpenAsync(googleClient, CurrentSaveName, true);

        private async void SaveSnapshotAsync(byte[] data)
        {
	        // Try to open snapshot
	        var result = await OpenSnapshot();

	        // Try to write it
			if (result.Snapshot != null)
				await CommitSnapshotAsync(result.Snapshot, data);
		}
		
		private async Task<ISnapshotsCommitSnapshotResult> CommitSnapshotAsync(ISnapshot snapshot, byte[] data)
		{
			// Write data to snapshot
			snapshot.SnapshotContents.WriteBytes(data);

			// Save snapshot
			var metadata = new SnapshotMetadataChangeBuilder()
				.SetDescription($"Modified at {DateTime.Now}")
				.Build();

			return await GamesClass.Snapshots.CommitAndCloseAsync(googleClient, snapshot, metadata);
		}
		
        public byte[] Snapshot
        {
            get => LoadFromSnapshot().Result.ReadFully();
            set => SaveSnapshotAsync(value);
        }
    }
}